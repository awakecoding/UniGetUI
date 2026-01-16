using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UniGetUI.Core.Tools;
using UniGetUI.Pages.SettingsPages.GeneralPages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class SettingsHomepage : UserControl, ISettingsPage
    {
        public bool CanGoBack => false;
        public string ShortTitle => CoreTools.Translate("WingetUI Settings");

        public event EventHandler? RestartRequired;

        public event EventHandler<Type>? NavigationRequested;

        public SettingsHomepage()
        {
            this.InitializeComponent();
        }
        public void Administrator(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Administrator));
        public void Backup(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Backup));
        public void Experimental(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Experimental));
        public void General(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(General));
        public void Interface(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Interface_P));
        public void Notifications(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Notifications));
        public void Operations(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Operations));
        public void Startup(object s, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Updates));
        private void Internet(object sender, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(Internet));
        private void ManagersShortcut(object sender, Avalonia.Interactivity.RoutedEventArgs e) => NavigationRequested?.Invoke(this, typeof(ManagersHomepage));
    }
}
