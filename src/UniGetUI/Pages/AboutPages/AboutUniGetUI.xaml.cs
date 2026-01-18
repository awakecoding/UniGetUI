using Avalonia.Controls;
using UniGetUI.Core.Data;
using UniGetUI.Core.Tools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Pages.AboutPages
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class AboutUniGetUI : UserControl
    {
        // XAML control declarations (TODO: Generate from XAML after proper Avalonia migration)
        private object? WebView = null; // TODO: Implement WebView
        public AboutUniGetUI()
        {
            InitializeComponent();
            VersionText.Text = CoreTools.Translate("You have installed WingetUI Version {0}", CoreData.VersionName);
            // TODO: Avalonia - DisclaimerBanner control properties
            // DisclaimerBanner.Title = CoreTools.Translate("Disclaimer");
            // DisclaimerBanner.Message = CoreTools.Translate("UniGetUI is not related to any of the compatible package managers. UniGetUI is an independent project.");
        }
    }
}
