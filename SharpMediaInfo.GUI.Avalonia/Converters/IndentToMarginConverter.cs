using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace SharpMediaInfo.GUI.Avalonia.Converters;

/// <summary>
/// Converts indent level to left margin
/// </summary>
public class IndentToMarginConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int indentLevel)
        {
            return new Thickness(indentLevel * 20, 2, 0, 0);
        }
        return new Thickness(0, 2, 0, 0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
