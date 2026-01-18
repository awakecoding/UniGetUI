using UniGetUI.Core.SettingsEngine;

namespace UniGetUI.Interface.Pages.LogPage
{
    public partial class OperationHistoryPage : BaseLogPage
    {
        public OperationHistoryPage() : base(false)
        {

        }

        public override void LoadLog(bool isReload = false)
        {
            // TODO: Avalonia - TextBox.Blocks doesn't exist, building plain text instead
            // Avalonia.Controls.Documents.Paragraph paragraph = new();
            string logText = "";
            foreach (string line in Settings.GetValue(Settings.K.OperationHistory).Split("\n"))
            {
                if (line.Replace("\r", "").Replace("\n", "").Trim() == "")
                {
                    continue;
                }

                // paragraph.Inlines.Add(new Avalonia.Controls.Documents.Run { Text = line.Replace("\r", "").Replace("\n", "") });
                // paragraph.Inlines.Add(new LineBreak());
                logText += line.Replace("\r", "").Replace("\n", "") + "\n";
            }
            // TODO: Avalonia - TextBox.Blocks doesn't exist, using Text property instead
            // LogTextBox.Blocks.Clear();
            // LogTextBox.Blocks.Add(paragraph);
            LogTextBox.Text = logText;

        }

        protected override void LoadLogLevels()
        {
            throw new NotImplementedException();
        }
    }
}
