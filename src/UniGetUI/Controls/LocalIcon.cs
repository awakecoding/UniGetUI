using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using UniGetUI.Interface.Enums;
using System;
using System.ComponentModel;
using System.Globalization;

namespace UniGetUI.Interface.Widgets
{
    [TypeConverter(typeof(IconTypeConverter))]
    public partial class LocalIcon : TextBlock
    {
        public static FontFamily font = (FontFamily)Application.Current.Resources["SymbolFont"];

        // Define as Avalonia StyledProperty for proper binding support
        public static readonly StyledProperty<IconType> IconProperty =
            AvaloniaProperty.Register<LocalIcon, IconType>(nameof(Icon));

        public IconType Icon
        {
            get => GetValue(IconProperty);
            set
            {
                SetValue(IconProperty, value);
                Text = $"{(char)value}";
            }
        }

        public LocalIcon()
        {
            FontFamily = font;
        }

        public LocalIcon(IconType icon) : this()
        {
            Text = $"{(char)icon}";
        }
    }

    // Type converter to handle string to IconType enum conversion in XAML
    public class IconTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                // Try to parse the string as an IconType enum
                if (Enum.TryParse<IconType>(stringValue, true, out var iconType))
                {
                    return iconType;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public static class IconBuilder
    {
        private static FontFamily customFont = null!;
        private static FontFamily symbolFont = null!;

        public static IconType SetIcon(this TextBlock block, IconType icon)
        {
            customFont ??= (FontFamily)Application.Current.Resources["SymbolFont"];
            block.Text = $"{(char)icon}";
            block.FontFamily = customFont;
            return icon;
        }

        public static IconType GetIcon(this TextBlock block)
        {
            return IconType.Help;
        }

        public static string SetGlyph(this TextBlock block, string glyph)
        {
            symbolFont ??= new FontFamily("Segoe Fluent Icons,Segoe MDL2 Assets");
            block.Text = glyph;
            block.FontFamily = symbolFont;
            return glyph;
        }

        public static string GetGlyph(this TextBlock block)
        {
            return block.Text;
        }
    }

    public partial class LocalIconSource : object
    {
        public static FontFamily font = (FontFamily)Application.Current.Resources["SymbolFont"];
        
        public string Glyph { get; set; } = "";
        public FontFamily FontFamily { get; set; } = null!;

        public IconType Icon
        {
            set => Glyph = $"{(char)value}";
        }

        public LocalIconSource()
        {
            FontFamily = font;
        }

        public LocalIconSource(IconType icon) : this()
        {
            Glyph = $"{(char)icon}";
        }
    }
}
