using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UniGetUI.Core.Tools;
using UniGetUI.Core.SettingsEngine;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Pages.SettingsPages.GeneralPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class Notifications : UserControl, ISettingsPage
    {
        public Notifications()
        {
            this.InitializeComponent();
        }

        protected virtual void OnNavigatedTo(object e)
        {
            if (Settings.Get(Settings.K.DisableSystemTray))
            {
                ToolbarText.IsVisible = true;
                DisableNotifications.IsEnabled = false;
                DisableUpdatesNotifications.IsEnabled = false;
                DisableErrorNotifications.IsEnabled = false;
                DisableSuccessNotifications.IsEnabled = false;
                DisableProgressNotifications.IsEnabled = false;
            }
            else
            {
                ToolbarText.IsVisible = false;
                DisableNotifications.IsEnabled = true;
                DisableUpdatesNotifications.IsEnabled = true;
                DisableErrorNotifications.IsEnabled = true;
                DisableSuccessNotifications.IsEnabled = true;
                DisableProgressNotifications.IsEnabled = true;
            }
            base.OnNavigatedTo(e);
        }

        public bool CanGoBack => true;
        public string ShortTitle => CoreTools.Translate("Notification preferences");

        public event EventHandler? RestartRequired;
        public event EventHandler<Type>? NavigationRequested;
    }
}
