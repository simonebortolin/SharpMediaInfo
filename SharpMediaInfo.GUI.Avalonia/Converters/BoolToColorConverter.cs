using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace SharpMediaInfo.GUI.Avalonia.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string colorPair)
        {
            var colors = colorPair.Split('|');
            if (colors.Length == 2)
            {
                var colorStr = boolValue ? colors[0] : colors[1];
                return Brush.Parse(colorStr);
            }
        }
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
