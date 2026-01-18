using Avalonia;
using Avalonia.Media;
using UniGetUI.Core.Tools;
using UniGetUI.PackageEngine;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.ManagerClasses.Classes;

namespace UniGetUI.Interface.Pages.LogPage
{
    public partial class ManagerLogsPage : BaseLogPage
    {

        public ManagerLogsPage() : base(true)
        {

        }

        public void LoadForManager(IPackageManager manager)
        {
            // TODO: Avalonia - ActualTheme not available, assuming dark theme
            bool IS_DARK = true; // this.ActualTheme == ElementTheme.Dark;
            bool verbose = LogLevelCombo.SelectedValue?.ToString()?.Contains(CoreTools.Translate("Verbose")) ?? false;

            if (!verbose) SelectLogLevelByName(manager.DisplayName);

            IManagerLogger TaskLogger = manager.TaskLogger;
            // TODO: Avalonia - TextBox.Blocks doesn't exist, needs alternative approach
            // LogTextBox.Blocks.Clear();
            string logText = $"Manager {manager.DisplayName} with version:\n{manager.Status.Version}\n\n——————————————————————————————————————————\n\n";
            // Avalonia.Controls.Documents.Paragraph versionParagraph = new();
            // versionParagraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = $"Manager {manager.DisplayName} with version:\n" });
            // versionParagraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = manager.Status.Version });
            // versionParagraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = "\n\n——————————————————————————————————————————\n\n" });
            // LogTextBox.Blocks.Add(versionParagraph);

            foreach (ITaskLogger operation in TaskLogger.Operations)
            {
                // Avalonia.Controls.Documents.Paragraph p = new();
                foreach (string line in operation.AsColoredString(verbose))
                {
                    Brush color = line[0] switch
                    {
                        '0' => new SolidColorBrush { Color = IS_DARK ? DARK_WHITE : LIGHT_WHITE },
                        '1' => new SolidColorBrush { Color = IS_DARK ? DARK_LIGHT_GREY : LIGHT_LIGHT_GREY },
                        '2' => new SolidColorBrush { Color = IS_DARK ? DARK_RED : LIGHT_RED },
                        '3' => new SolidColorBrush { Color = IS_DARK ? DARK_BLUE : LIGHT_BLUE },
                        '4' => new SolidColorBrush { Color = IS_DARK ? DARK_GREEN : LIGHT_GREEN },
                        '5' => new SolidColorBrush { Color = IS_DARK ? DARK_YELLOW : LIGHT_YELLOW },
                        _ => new SolidColorBrush { Color = IS_DARK ? DARK_YELLOW : LIGHT_YELLOW },
                    };
                    // TODO: Avalonia - Run.Foreground not supported, building plain text instead
                    // p.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line[1..] + "\n" });
                    logText += line[1..] + "\n";
                }
                // TODO: Avalonia - TextBox.Blocks doesn't exist, using Text property instead
                // ((Avalonia.Controls.Documents.Run)p.Inlines[^1]).Text = ((Avalonia.Controls.Documents.Run)p.Inlines[^1]).Text.TrimEnd();
                // LogTextBox.Blocks.Add(p);
            }
            LogTextBox.Text = logText;
        }

        public override void LoadLog(bool isReload = false)
        {
            foreach (IPackageManager manager in PEInterface.Managers)
            {
                if (LogLevelCombo.SelectedValue?.ToString()?.Contains(manager.DisplayName) ?? false)
                {
                    LoadForManager(manager);
                    break;
                }

                // TODO: Avalonia - ScrollViewer methods don't exist, comment out for now
                // if (isReload) MainScroller.ScrollToVerticalOffset(MainScroller.ScrollableHeight);
            }
        }

        protected override void LoadLogLevels()
        {
            LogLevelCombo.Items.Clear();
            foreach (IPackageManager manager in PEInterface.Managers)
            {
                LogLevelCombo.Items.Add(manager.DisplayName);
                LogLevelCombo.Items.Add($"{manager.DisplayName} ({CoreTools.Translate("Verbose")})");
            }
            LogLevelCombo.SelectedIndex = 0;
        }
    }
}
