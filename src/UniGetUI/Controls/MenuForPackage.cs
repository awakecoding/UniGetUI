
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using UniGetUI.Core.Tools;
using UniGetUI.Interface.Enums;

namespace UniGetUI.Interface.Widgets
{
    public partial class BetterMenu : MenuFlyout
    {
        private readonly Avalonia.Styling.Style menuyStyle = (Avalonia.Styling.Style)Application.Current.Resources["BetterContextMenu"];
        public BetterMenu()
        {
            // TODO: Avalonia - MenuFlyoutPresenterStyle doesn't exist in Avalonia
            // MenuFlyoutPresenterStyle = menuyStyle;
        }
    }

    public partial class BetterMenuItem : MenuItem
    {
        private readonly Avalonia.Styling.Style menuStyle = (Avalonia.Styling.Style)Application.Current.Resources["BetterMenuItem"];

        public IconType IconName
        {
            set
            {
                var icon = new LocalIcon(value) { FontSize = 24 };
                Icon = icon;
            }
        }

        public string UntranslatedText { set => base.Header = value; }
        // TODO: Avalonia - MenuItem.Text changed to Header in Avalonia
        public new string Text { set => base.Header = CoreTools.Translate(value); }

        public BetterMenuItem()
        {
            // TODO: Avalonia - Style assignment not supported in constructor
            // Avalonia.Styling.Style = menuStyle;
        }
    }

    public partial class BetterToggleMenuItem : MenuItem
    {
        private readonly Avalonia.Styling.Style menuStyle = (Avalonia.Styling.Style)Application.Current.Resources["BetterToggleMenuItem"];

        public IconType IconName
        {
            set
            {
                var icon = new LocalIcon(value) { FontSize = 24 };
                Icon = icon;
            }
        }

        // TODO: Avalonia - MenuItem.Text changed to Header in Avalonia
        public new string Text
        {
            set => base.Header = CoreTools.Translate(value);
        }

        public BetterToggleMenuItem()
        {
            // TODO: Avalonia - Style assignment not supported in constructor
            // Avalonia.Styling.Style = menuStyle;
        }
    }

    public partial class BetterTabViewItem : TabItem
    {
        string line1 = "";
        string line2 = "";
        public string Line1 { set { line1 = value; LoadText(); } }
        public string Line2 { set { line2 = value; LoadText(); } }

        // TODO: Avalonia - TabItem.IconSource doesn't exist, commenting out
        // public IconType IconName { set => IconSource = new LocalIconSource(value); }


        public BetterTabViewItem()
        {
            // TODO: Avalonia - TabItem.IsClosable and CanDrag don't exist in Avalonia
            // IsClosable = false;
            // CanDrag = false;
        }

        public void LoadText()
        {
            string text = "";
            // The invisible U+200E character here is used to prevent the text from being
            // trimmed in the TabItem header, adding a little bit of padding at the end.
            if (line1 != "") text += CoreTools.Translate(line1) + " ‎‎ ";
            if (line2 != "") text += (text.Length> 0?"\n":"") + CoreTools.Translate(line2) + " ‎ ";
            Header = text;
        }
    }

    public partial class BetterFlyout: Flyout
    {
        public BetterFlyout() : base()
        {
            // TODO: Avalonia - Flyout properties don't exist in Avalonia (WinUI-specific)
            // ShouldConstrainToRootBounds = false;
            // SystemBackdrop = new DesktopAcrylicBackdrop();
            // FlyoutPresenterStyle = (Avalonia.Styling.Style)Application.Current.Resources["BetterFlyoutPresenterStyle"];
        }
    }

}
