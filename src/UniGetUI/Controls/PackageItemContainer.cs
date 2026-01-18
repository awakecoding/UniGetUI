using Avalonia;
using Avalonia.Controls;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.PackageClasses;

namespace UniGetUI.Interface.Widgets
{
    public partial class PackageItemContainer : ContentControl
    {
        public static readonly StyledProperty<IPackage?> PackageProperty =
            AvaloniaProperty.Register<PackageItemContainer, IPackage?>(nameof(Package));

        public IPackage? Package
        {
            get => GetValue(PackageProperty);
            set => SetValue(PackageProperty, value);
        }

        public static readonly StyledProperty<PackageWrapper> WrapperProperty =
            AvaloniaProperty.Register<PackageItemContainer, PackageWrapper>(nameof(Wrapper));

        public PackageWrapper Wrapper
        {
            get => GetValue(WrapperProperty);
            set => SetValue(WrapperProperty, value);
        }
    }
}
