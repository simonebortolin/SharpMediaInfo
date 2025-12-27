using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SharpMediaInfo.GUI.Avalonia.Converters;

public class BoolToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramStr)
        {
            var parts = paramStr.Split('|');
            if (parts.Length == 2)
            {
                return boolValue ? parts[0] : parts[1];
            }
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // One-way converter, cannot convert back
        return BindingOperations.DoNothing;
    }
}
