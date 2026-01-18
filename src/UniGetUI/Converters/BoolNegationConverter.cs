using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace UniGetUI.Converters
{
    /// <summary>
    /// Inverts a boolean value.
    /// </summary>
    public class BoolNegationConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
}
