using Avalonia.Media;
using UniGetUI.Core.Logging;
using UniGetUI.Core.Tools;

namespace UniGetUI.Interface.Pages.LogPage
{
    public partial class UniGetUILogPage : BaseLogPage
    {
        public UniGetUILogPage() : base(true)
        {
        }

        protected override void LoadLogLevels()
        {
            LogLevelCombo.Items.Clear();
            LogLevelCombo.Items.Add(CoreTools.Translate("1 - Errors"));
            LogLevelCombo.Items.Add(CoreTools.Translate("2 - Warnings"));
            LogLevelCombo.Items.Add(CoreTools.Translate("3 - Information (less)"));
            LogLevelCombo.Items.Add(CoreTools.Translate("4 - Information (more)"));
            LogLevelCombo.Items.Add(CoreTools.Translate("5 - information (debug)"));
            LogLevelCombo.SelectedIndex = 3;
#if DEBUG
            LogLevelCombo.SelectedIndex = 4;
#endif
        }

        public override void LoadLog(bool isReload = false)
        {
            // TODO: Avalonia - ActualTheme not available, assuming dark theme
            bool IS_DARK = true; // ActualTheme == Microsoft.UI.Xaml.ElementTheme.Dark;

            LogEntry[] logs = Logger.GetLogs();
            // TODO: Avalonia - TextBox.Blocks doesn't exist, needs alternative approach
            // LogTextBox.Blocks.Clear();
            string logText = "";
            foreach (LogEntry log_entry in logs)
            {
                Avalonia.Controls.Documents.Paragraph p = new();
                if (log_entry.Content == "")
                {
                    continue;
                }

                if (LOG_LEVEL == 1 && (log_entry.Severity == LogEntry.SeverityLevel.Debug || log_entry.Severity == LogEntry.SeverityLevel.Info || log_entry.Severity == LogEntry.SeverityLevel.Success || log_entry.Severity == LogEntry.SeverityLevel.Warning))
                {
                    continue;
                }

                if (LOG_LEVEL == 2 && (log_entry.Severity == LogEntry.SeverityLevel.Debug || log_entry.Severity == LogEntry.SeverityLevel.Info || log_entry.Severity == LogEntry.SeverityLevel.Success))
                {
                    continue;
                }

                if (LOG_LEVEL == 3 && (log_entry.Severity == LogEntry.SeverityLevel.Debug || log_entry.Severity == LogEntry.SeverityLevel.Info))
                {
                    continue;
                }

                if (LOG_LEVEL == 4 && (log_entry.Severity == LogEntry.SeverityLevel.Debug))
                {
                    continue;
                }

                Brush color = log_entry.Severity switch
                {
                    LogEntry.SeverityLevel.Debug => new SolidColorBrush { Color = IS_DARK ? DARK_GREY : LIGHT_GREY },
                    LogEntry.SeverityLevel.Info => new SolidColorBrush { Color = IS_DARK ? DARK_LIGHT_GREY : LIGHT_LIGHT_GREY },
                    LogEntry.SeverityLevel.Success => new SolidColorBrush { Color = IS_DARK ? DARK_WHITE : LIGHT_WHITE },
                    LogEntry.SeverityLevel.Warning => new SolidColorBrush { Color = IS_DARK ? DARK_YELLOW : LIGHT_YELLOW },
                    LogEntry.SeverityLevel.Error => new SolidColorBrush { Color = IS_DARK ? DARK_RED : LIGHT_RED },
                    _ => new SolidColorBrush { Color = IS_DARK ? DARK_GREY : LIGHT_GREY },
                };
                string[] lines = log_entry.Content.Split('\n');
                int date_length = -1;
                foreach (string line in lines)
                {
                    if (date_length == -1)
                    {
                        // TODO: Avalonia - Run.Foreground not supported, building plain text instead
                        // p.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = $"[{log_entry.Time}] {line}\n" });
                        logText += $"[{log_entry.Time}] {line}\n";
                        date_length = $"[{log_entry.Time}] ".Length;
                    }
                    else
                    {
                        // TODO: Avalonia - Run.Foreground not supported, building plain text instead
                        // p.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = new string(' ', date_length) + line + "\n" });
                        logText += new string(' ', date_length) + line + "\n";
                    }
                }
                // TODO: Avalonia - TextBox.Blocks doesn't exist, using Text property instead
                // ((Avalonia.Controls.Documents.Run)p.Inlines[^1]).Text = ((Avalonia.Controls.Documents.Run)p.Inlines[^1]).Text.TrimEnd();
                // LogTextBox.Blocks.Add(p);
            }
            LogTextBox.Text = logText;
            // TODO: Avalonia - ScrollViewer methods don't exist, comment out for now
            // if (isReload) MainScroller.ScrollToVerticalOffset(MainScroller.ScrollableHeight);
        }
    }
}
