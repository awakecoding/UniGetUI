using Avalonia.Controls;
using System.Diagnostics;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UniGetUI.Interface.Pages.AboutPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class SupportMe : UserControl
    {
        public SupportMe()
        {
            InitializeComponent();
        }

        private void KoFiButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://ko-fi.com/martinet101") { UseShellExecute = true });
        }

        private void HomepageButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.marticliment.com/unigetui") { UseShellExecute = true });
        }
    }
}
