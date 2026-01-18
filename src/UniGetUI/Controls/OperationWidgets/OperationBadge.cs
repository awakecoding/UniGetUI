using UniGetUI.Interface.Enums;

namespace UniGetUI.Controls.OperationWidgets;
public class OperationBadge
{
    public string Tooltip { get; set; }
    public string PrimaryBanner { get; set; }
    public string SecondaryBanner { get; set; }
    public bool SecondaryBannerVisible { get; set; }
    public IconType Icon { get; set; }

    public OperationBadge(string tooltip, IconType icon, string primaryBanner, string? secondaryBanner = null)
    {
        Tooltip = tooltip;
        Icon = icon;
        PrimaryBanner = primaryBanner;
        if (secondaryBanner is null || secondaryBanner == String.Empty)
        {
            SecondaryBannerVisible = false;
            SecondaryBanner = "";
        }
        else
        {
            SecondaryBanner = secondaryBanner;
            SecondaryBannerVisible = true;
        }
    }
}
