using Avalonia.Controls;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.PackageClasses;

namespace UniGetUI.Interface.Widgets
{
    public partial class PackageItemContainer : ContentControl
    {
        public IPackage? Package { get; set; }
        public PackageWrapper Wrapper { get; set; } = null!;
    }
}
