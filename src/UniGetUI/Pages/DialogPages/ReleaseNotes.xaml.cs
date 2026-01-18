using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UniGetUI.Core.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class ReleaseNotes : UserControl, IDisposable
    {
        public event EventHandler<EventArgs>? Close;

        // XAML control declarations (TODO: Generate from XAML after proper Avalonia migration)
        private object? WebView = null; // TODO: Implement WebView
        public ReleaseNotes()
        {
            InitializeComponent();
            _ = InitializeWebView();

            // TODO: Avalonia - WebView2 properties (Windows-specific)
            // WebView.NavigationStarting += (_, _) => { ProgressBar.IsVisible = true; };
            // WebView.NavigationCompleted += (_, _) => { ProgressBar.IsVisible = false; };
        }

        private async Task InitializeWebView()
        {
            // TODO: Avalonia - WebView2 methods (Windows-specific)
            // await WebView.EnsureCoreWebView2Async();
            // WebView.Source = new Uri("https://github.com/marticliment/WingetUI/releases/tag/" + CoreData.VersionName);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            // TODO: Avalonia - WebView2.Close method (Windows-specific)
            // WebView.Close();
        }

        private void CloseButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close?.Invoke(this, EventArgs.Empty);
        }
    }
}
