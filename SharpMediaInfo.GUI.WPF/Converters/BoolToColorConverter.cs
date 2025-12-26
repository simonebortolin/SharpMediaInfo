using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SharpMediaInfo.GUI.WPF.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && parameter is string param)
        {
            var parts = param.Split('|');
            if (parts.Length == 2)
            {
                var colorName = b ? parts[0] : parts[1];
                return colorName.ToLower() switch
                {
                    "green" => Brushes.Green,
                    "red" => Brushes.Red,
                    "gray" => Brushes.Gray,
                    "orange" => Brushes.Orange,
                    _ => Brushes.Black
                };
            }
        }
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
