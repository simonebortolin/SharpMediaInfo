using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SharpMediaInfo.GUI.Avalonia.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            bool invert = parameter is string s && s == "Invert";
            return invert ? !boolValue : boolValue;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            bool invert = parameter is string s && s == "Invert";
            return invert ? !boolValue : boolValue;
        }
        return false;
    }
}
