using Avalonia.Controls;

namespace Avalonia.Controls.Documents
{
    // Stub implementation of Paragraph for migration
    // Avalonia doesn't have a direct equivalent
    // This should be replaced with proper text formatting later
    public class Paragraph : TextBlock
    {
        public Paragraph()
        {
            TextWrapping = Media.TextWrapping.Wrap;
        }
    }
    
    // Stub for Run (inline text element)
    public class Run : Control
    {
        public string Text { get; set; } = "";
    }
}
