using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace SharpMediaInfo.GUI.Avalonia.Converters;

public class BoolToFontStyleConverter : IValueConverter
{
    public static readonly BoolToFontStyleConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? FontStyle.Italic : FontStyle.Normal;
        }
        return FontStyle.Normal;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
