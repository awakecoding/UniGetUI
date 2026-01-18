using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Interactivity;
using UniGetUI.Core.Tools;
using UniGetUI.Interface.Pages.AboutPages;
using UniGetUI.Interface;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public partial class AboutUniGetUI : UserControl
    {

        public event EventHandler? Close;
        private int previousSelectedIndex;
        public AboutUniGetUI()
        {
            InitializeComponent();
            // TODO: Avalonia - TabItem.Text changed to Header in Avalonia
            SelectorBarItemPage1.Header = CoreTools.Translate("About");
            SelectorBarItemPage2.Header = CoreTools.Translate("Third-party licenses");
            SelectorBarItemPage3.Header = CoreTools.Translate("Contributors");
            SelectorBarItemPage4.Header = CoreTools.Translate("Translators");
            SelectorBarItemPage5.Header = CoreTools.Translate("Support me");
        }

        private void SelectorBar_SelectionChanged(object? sender, SelectionChangedEventArgs args)
        {
            if (sender is not TabControl tabControl) return;
            
            // TODO: Avalonia - SelectorBarItem type doesn't exist, using object
            object selectedItem = tabControl.SelectedItem;
            int currentSelectedIndex = tabControl.Items.IndexOf(selectedItem);
            Type pageType = currentSelectedIndex switch
            {
                0 => typeof(Pages.AboutPages.AboutUniGetUI),
                1 => typeof(ThirdPartyLicenses),
                2 => typeof(Contributors),
                3 => typeof(Translators),
                _ => typeof(SupportMe),
            };
            SlideNavigationTransitionEffect slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = currentSelectedIndex;

        }

        private void CloseButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close?.Invoke(this, EventArgs.Empty);
        }
    }
}
