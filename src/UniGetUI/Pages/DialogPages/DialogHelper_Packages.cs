using System.Web;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using UniGetUI.Core.Logging;
using UniGetUI.Core.Tools;
using UniGetUI.Interface.Dialogs;
using UniGetUI.Interface.Enums;
using UniGetUI.Interface.Telemetry;
using UniGetUI.PackageEngine;
using UniGetUI.PackageEngine.Enums;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.PackageClasses;
using UniGetUI.PackageEngine.PackageLoader;
using UniGetUI.PackageEngine.Serializable;
using UniGetUI.Pages.SettingsPages.GeneralPages;
// TODO: Avalonia - Using project's ContentDialogResult enum
using ContentDialogResult = UniGetUI.Interface.ContentDialogResult;

namespace UniGetUI.Pages.DialogPages;

public static partial class DialogHelper
{
    /// <summary>
    /// Will update the Installation Options for the given Package, and will return whether the user choose to continue
    /// </summary>
    public static async Task<bool> ShowInstallatOptions_Continue(IPackage package, OperationType operation)
    {
        var options = await InstallOptionsFactory.LoadForPackageAsync(package);
        var (dialogOptions, dialogResult) = await ShowInstallOptions(package, operation, options);

        // TODO: Avalonia - Convert bool? to ContentDialogResult check
        if (dialogResult == true || dialogResult == false)
        {
            Logger.Debug($"Saving updated options for package {package.Id}");
            await InstallOptionsFactory.SaveForPackageAsync(dialogOptions, package);
        }
        else
        {
            Logger.Debug($"Install options dialog for {package.Id} was canceled, no changes will be saved");
        }

        return dialogResult == false; // Secondary button (false) means continue
    }

    /// <summary>
    /// Will update the Installation Options for the given imported package
    /// </summary>
    public static async Task<bool?> ShowInstallOptions_ImportedPackage(ImportedPackage importedPackage)
    {
        var (options, dialogResult) =
            await ShowInstallOptions(importedPackage, OperationType.None, importedPackage.installation_options.Copy());

        // TODO: Avalonia - dialogResult is bool?, not ContentDialogResult
        if (dialogResult.HasValue)
        {
            importedPackage.installation_options = options;
            importedPackage.FirePackageVersionChangedEvent();
        }

        return dialogResult;
    }

    private static async Task<(InstallOptions, bool?)> ShowInstallOptions(
        IPackage package,
        OperationType operation,
        InstallOptions options)
    {
        InstallOptionsPage OptionsPage = new(package, operation, options);

        Window OptionsDialog = DialogFactory.Create_AsWindow(true, true);

        // TODO: Avalonia - OptionsDialog.SecondaryButtonText = operation switch { ... };
        // TODO: Avalonia - OptionsDialog.PrimaryButtonText = CoreTools.Translate("Save and close");
        // TODO: Avalonia - OptionsDialog.DefaultButton = ContentDialogButton.Secondary;
        // OptionsDialog.Title = CoreTools.Translate("{0} installation options", package.Name);
        OptionsDialog.Content = OptionsPage;

        OptionsPage.Close += (_, _) => { OptionsDialog.Hide(); };

        ContentDialogResult result = await ShowDialogAsync(OptionsDialog);
        return (await OptionsPage.GetUpdatedOptions(), result == ContentDialogResult.Primary ? true : (result == ContentDialogResult.Secondary ? false : (bool?)null));
    }

    public static async Task ShowPackageDetails(IPackage package, OperationType operation, TEL_InstallReferral referral)
    {
        PackageDetailsPage DetailsPage = new(package, operation, referral);

        Window DetailsDialog = DialogFactory.Create_AsWindow(false);
        DetailsDialog.Content = DetailsPage;
        DetailsPage.Close += (_, _) => { DetailsDialog.Hide(); };

        await ShowDialogAsync(DetailsDialog);
    }

    public static async Task<bool> ConfirmUninstallation(IPackage package)
    {
        Window dialog = DialogFactory.Create();
        dialog.Title = CoreTools.Translate("Are you sure?");
        // TODO: Avalonia - dialog.PrimaryButtonText = CoreTools.Translate("Yes");
        // TODO: Avalonia - dialog.SecondaryButtonText = CoreTools.Translate("No");
        // TODO: Avalonia - dialog.DefaultButton = ContentDialogButton.Secondary;
        dialog.Content = CoreTools.Translate("Do you really want to uninstall {0}?", package.Name);

        // TODO: Avalonia - ShowDialogAsync returns ContentDialogResult, check for Primary
        return await ShowDialogAsync(dialog) == ContentDialogResult.Primary;
    }

    public static async Task<bool> ConfirmUninstallation(IReadOnlyList<IPackage> packages)
    {
        if (!packages.Any())
        {
            return false;
        }

        if (packages.Count == 1)
        {
            return await ConfirmUninstallation(packages[0]);
        }

        Window dialog = DialogFactory.Create();
        dialog.Title = CoreTools.Translate("Are you sure?");
        // TODO: Avalonia - dialog.PrimaryButtonText = CoreTools.Translate("Yes");
        // TODO: Avalonia - dialog.SecondaryButtonText = CoreTools.Translate("No");
        // TODO: Avalonia - dialog.DefaultButton = ContentDialogButton.Secondary;


        StackPanel p = new();
        p.Children.Add(new TextBlock
        {
            Text = CoreTools.Translate("Do you really want to uninstall the following {0} packages?",
                packages.Count),
            Margin = new Thickness(0, 0, 0, 5)
        });

        string pkgList = "";
        foreach (IPackage package in packages)
        {
            pkgList += " ● " + package.Name + "\x0a";
        }

        TextBlock PackageListTextBlock =
            new() { FontFamily = new FontFamily("Consolas"), Text = pkgList };
        // TODO: Avalonia - Use ScrollViewer instead of ScrollView
        p.Children.Add(new ScrollViewer { Content = PackageListTextBlock, MaxHeight = 200 });

        dialog.Content = p;

        // TODO: Avalonia - ShowDialogAsync returns ContentDialogResult, check for Primary
        return await ShowDialogAsync(dialog) == ContentDialogResult.Primary;
    }

    public static void ShowSharedPackage_ThreadSafe(string id, string combinedSourceName)
    {
        var contents = combinedSourceName.Split(':');
        string managerName = contents[0];
        string sourceName = "";
        if (contents.Length > 1) sourceName = contents[1];
        _ = GetPackageFromIdAndManager(id, managerName, sourceName, "LEGACY_COMBINEDSOURCE");
    }

    public static void ShowSharedPackage_ThreadSafe(string id, string managerName, string sourceName)
    {
        // TODO: Avalonia - Use Avalonia.Threading.Dispatcher.UIThread directly
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _ = GetPackageFromIdAndManager(id, managerName, sourceName, "DEFAULT");
        });
    }

    private static async Task GetPackageFromIdAndManager(string id, string managerName, string sourceName, string eventSource)
    {
        int loadingId = ShowLoadingDialog(CoreTools.Translate("Please wait..."));
        try
        {
            Window.Activate();

            var findResult = await Task.Run(() => DiscoverablePackagesLoader.Instance.GetPackageFromIdAndManager(id, managerName, sourceName));

            HideLoadingDialog(loadingId);

            if (findResult.Item1 is null) throw new KeyNotFoundException(findResult.Item2 ?? "Unknown error");

            TelemetryHandler.SharedPackage(findResult.Item1, eventSource);
            _ = ShowPackageDetails(findResult.Item1, OperationType.Install, TEL_InstallReferral.FROM_WEB_SHARE);

        }
        catch (Exception ex)
        {
            Logger.Error($"An error occurred while attempting to show the package with id {id}");
            HideLoadingDialog(loadingId);

            var warningDialog = new Window
            {
                Title = CoreTools.Translate("Package not found"),
                Content = CoreTools.Translate("An error occurred when attempting to show the package with Id {0}", id) + ":\n" + ex.Message,
                // TODO: Avalonia - Window.CloseButtonText doesn't exist (WinUI-specific property)
                // CloseButtonText = CoreTools.Translate("Ok"),
                // TODO: Avalonia - Window.DefaultButton doesn't exist (WinUI-specific property)
                // DefaultButton = ContentDialogButton.Close,
                // TODO: Avalonia - Window.XamlRoot doesn't exist (WinUI-specific property)
                // XamlRoot = MainApp.Instance.MainWindow.Content.XamlRoot // Ensure the dialog is shown in the correct context
            };

            await ShowDialogAsync(warningDialog);

        }
    }

    public static async Task ShowBundleSecurityReport(Dictionary<string, List<BundleReportEntry>> packageReport)
    {
        var dialog = DialogFactory.Create_AsWindow(true, true);
        Brush bad = new SolidColorBrush(Colors.PaleVioletRed);
        Brush good = new SolidColorBrush(Colors.Gold);

        // TODO: Implement theme detection for Avalonia (MainWindow.NavigationPage.ActualTheme not available)
        // if (Window.NavigationPage.ActualTheme is ElementTheme.Light)
        // {
        //     bad = new SolidColorBrush(Colors.Red);
        //     good = new SolidColorBrush(Colors.DarkGoldenrod);
        // }

        var title = CoreTools.Translate("Bundle security report");
        dialog.Title = title;
        Button a;
        // Avalonia doesn't have RichTextBlock with Paragraph/Inlines - use simpler TextBlock approach
        StackPanel contentPanel = new StackPanel { Spacing = 8 };
        
        TextBlock headerText = new TextBlock
        {
            Text = CoreTools.Translate("This package bundle had some settings that are potentially dangerous, and may be ignored by default.") + "\n" +
                   " - " + CoreTools.Translate("Entries that show in YELLOW will be IGNORED.") + "\n" +
                   " - " + CoreTools.Translate("Entries that show in RED will be IMPORTED.") + "\n" +
                   CoreTools.Translate("You can change this behavior on UniGetUI security settings."),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            FontFamily = new Avalonia.Media.FontFamily("Consolas")
        };
        contentPanel.Children.Add(headerText);

        a = new Button
        {
            Content = CoreTools.Translate("Open UniGetUI security settings")
        };
        contentPanel.Children.Add(a);

        TextBlock detailsHeader = new TextBlock
        {
            Text = "\n" + CoreTools.Translate("Should you modify the security settings, you will need to open the bundle again for the changes to take effect.") + "\n\n" +
                   CoreTools.Translate("Details of the report:"),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
        contentPanel.Children.Add(detailsHeader);

        foreach(var pair in packageReport)
        {
            TextBlock packageText = new TextBlock
            {
                Text = $" - {CoreTools.Translate("Package")}: {pair.Key}:",
                FontFamily = new Avalonia.Media.FontFamily("Consolas")
            };
            contentPanel.Children.Add(packageText);

            foreach (var issue in pair.Value)
            {
                TextBlock issueText = new TextBlock
                {
                    Text = $"   * {issue.Line}",
                    FontFamily = new Avalonia.Media.FontFamily("Consolas"),
                    Foreground = issue.Allowed? bad: good
                };
                contentPanel.Children.Add(issueText);
            }
        }

        dialog.Content = new ScrollViewer()
        {
            MaxWidth = 800,
            Background = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"],
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Content = contentPanel
        };
        a.Click += (_, _) => {
            dialog.Hide();
            // TODO: Implement navigation - Window.NavigationPage not available
            // Window.NavigationPage.OpenSettingsPage(typeof(Administrator));
        };
        // dialog.SecondaryButtonText = CoreTools.Translate("Close"); // Window doesn't have this property
        await ShowDialogAsync(dialog);
    }

    public static void SharePackage(IPackage? package)
    {
        if (package is null)
            return;

        if (package.Source.IsVirtualManager || package is InvalidImportedPackage)
        {
            DialogHelper.ShowDismissableBalloon(
                CoreTools.Translate("Something went wrong"),
                CoreTools.Translate("\"{0}\" is a local package and can't be shared", package.Name)
            );
            return;
        }

#if WINDOWS
        IntPtr hWnd = MainApp.Instance.MainWindow.GetWindowHandle();

        NativeHelpers.IDataTransferManagerInterop interop =
            DataTransferManager.As<NativeHelpers.IDataTransferManagerInterop>();

        IntPtr result = interop.GetForWindow(hWnd, NativeHelpers._dtm_iid);
        DataTransferManager dataTransferManager = WinRT.MarshalInterface
            <DataTransferManager>.FromAbi(result);

        dataTransferManager.DataRequested += (_, args) =>
        {
            DataRequest dataPackage = args.Request;
            Uri ShareUrl = new("https://marticliment.com/unigetui/share?"
                               + "name=" + HttpUtility.UrlEncode(package.Name)
                               + "&id=" + HttpUtility.UrlEncode(package.Id)
                               + "&sourceName=" + HttpUtility.UrlEncode(package.Source.Name)
                               + "&managerName=" + HttpUtility.UrlEncode(package.Manager.DisplayName));

        dataPackage.Data.SetWebLink(ShareUrl);
        dataPackage.Data.Properties.Title = "Sharing " + package.Name;
        dataPackage.Data.Properties.ApplicationName = "WingetUI";
        dataPackage.Data.Properties.ContentSourceWebLink = ShareUrl;
        dataPackage.Data.Properties.Description = "Share " + package.Name + " with your friends";
        dataPackage.Data.Properties.PackageFamilyName = "WingetUI";
    };

        interop.ShowShareUIForWindow(hWnd);
#else
        // TODO: Avalonia - Implement cross-platform sharing
        DialogHelper.ShowDismissableBalloon(
            CoreTools.Translate("Sharing not supported"),
            CoreTools.Translate("Sharing is currently only supported on Windows")
        );
#endif
    }

    /// <summary>
    /// Returns true if the user confirms to lose unsaved changes, and wants to proceed with the creation of a new bundle
    /// </summary>
    /// <returns></returns>
    public static async Task<bool> AskLoseChangesAndCreateBundle()
    {
        // Avalonia doesn't have RichTextBlock - use StackPanel with TextBlocks
        var panel = new StackPanel { Spacing = 10 };
        panel.Children.Add(new TextBlock { Text = CoreTools.Translate("Are you sure you want to create a new package bundle? ") });
        panel.Children.Add(new TextBlock { 
            Text = CoreTools.Translate("Any unsaved changes will be lost"),
            FontWeight = Avalonia.Media.FontWeight.SemiBold
        });

        Window dialog = DialogFactory.Create();
        dialog.Title = CoreTools.Translate("Warning!");
        dialog.Content = panel;
        // TODO: Implement dialog button properties for Avalonia
        // dialog.DefaultButton = ContentDialogButton.Secondary;
        // dialog.PrimaryButtonText = CoreTools.Translate("Yes");
        // dialog.SecondaryButtonText = CoreTools.Translate("No");

        return await DialogHelper.ShowDialogAsync(dialog) is ContentDialogResult.Primary;
    }


}
