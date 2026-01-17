using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using UniGetUI.Controls.OperationWidgets;
using UniGetUI.Core.Tools;
using UniGetUI.Interface.Widgets;
using UniGetUI.PackageOperations;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Pages.DialogPages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public partial class OperationFailedDialog : UserControl
{
    public event EventHandler<EventArgs>? Close;
    Avalonia.Controls.Documents.Paragraph par;

    // XAML control declarations (TODO: Generate from XAML after proper Avalonia migration)
    private TextBlock _textblock = new();

    private static SolidColorBrush errorColor = null!;
    private static SolidColorBrush debugColor = null!;

    public OperationFailedDialog(AbstractOperation operation, OperationControl opControl)
    {
        this.InitializeComponent();

        errorColor ??= (SolidColorBrush)Application.Current.Resources["SystemFillColorCriticalBrush"];
        debugColor ??= (SolidColorBrush)Application.Current.Resources["SystemFillColorNeutralBrush"];

        headerContent.Text = $"{operation.Metadata.FailureMessage}.\n"
           + CoreTools.Translate("Please see the Command-line Output or refer to the Operation History for further information about the issue.");

        par = new Avalonia.Controls.Documents.Paragraph();
        foreach (var line in operation.GetOutput())
        {
            if (line.Item2 is AbstractOperation.LineType.Information)
            {
                par.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line.Item1 + "\x0a" });
            }
            else if (line.Item2 is AbstractOperation.LineType.VerboseDetails)
            {
                // TODO: Avalonia - Run.Foreground not supported, color removed
                par.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line.Item1 + "\x0a" });
            }
            else
            {
                // TODO: Avalonia - Run.Foreground not supported, color removed
                par.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line.Item1 + "\x0a" });
            }
        }

        CommandLineOutput.Blocks.Add(par);

        var CloseButton = new Button
        {
            Content = CoreTools.Translate("Close"),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            Height = 30,
        };
        CloseButton.Click += (_, _) => Close?.Invoke(this, EventArgs.Empty);

        Control _retryButton;

        var retryOptions = opControl.GetRetryOptions(() => Close?.Invoke(this, EventArgs.Empty));
        if (retryOptions.Count != 0)
        {
            SplitButton RetryButton = new SplitButton
            {
                Content = CoreTools.Translate("Retry"),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Height = 30,
            };
            RetryButton.Click += (_, _) =>
            {
                operation.Retry(AbstractOperation.RetryMode.Retry);
                Close?.Invoke(this, EventArgs.Empty);
            };
            BetterMenu menu = new();
            RetryButton.Flyout = menu;
            foreach (var opt in retryOptions)
            {
                menu.Items.Add(opt);
            }

            _retryButton = RetryButton;
        }
        else
        {
            var RetryButton = new Button
            {
                Content = CoreTools.Translate("Retry"),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Height = 30,
            };
            RetryButton.Click += (_, _) =>
            {
                operation.Retry(AbstractOperation.RetryMode.Retry);
                Close?.Invoke(this, EventArgs.Empty);
            };
            _retryButton = RetryButton;
        }

        ButtonsLayout.Children.Add(CloseButton);
        ButtonsLayout.Children.Add(_retryButton);
        Grid.SetColumn(CloseButton, 1);
    }

    private void CloseButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close?.Invoke(this, EventArgs.Empty);
    }
}
