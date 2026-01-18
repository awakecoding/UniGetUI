using Avalonia.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Avalonia.Controls.Documents
{
    // Stub implementation of Paragraph for migration
    // Avalonia doesn't have a direct equivalent
    // This should be replaced with proper text formatting later
    public class Paragraph : TextBlock
    {
        public ObservableCollection<Run> Inlines { get; }
        
        public Paragraph()
        {
            Inlines = new ObservableCollection<Run>();
            TextWrapping = Media.TextWrapping.Wrap;
        }
    }
    
    // Stub for Run (inline text element)
    public class Run : Control
    {
        public string Text { get; set; } = "";
    }
}

namespace Avalonia.Controls
{
    // Stub implementation of RichTextBlock for migration
    // Avalonia doesn't have RichTextBlock, this provides compatibility layer
    // TODO: Replace with proper Avalonia text rendering
    public class RichTextBlock : TextBlock
    {
        public ObservableCollection<Documents.Paragraph> Blocks { get; }

        public RichTextBlock()
        {
            Blocks = new ObservableCollection<Documents.Paragraph>();
            TextWrapping = Media.TextWrapping.Wrap;
        }
    }
}
