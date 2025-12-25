using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SharpMediaInfo.GUI.WPF.Converters;

/// <summary>
/// Converts boolean to Visibility with support for inversion and different visibility modes
/// </summary>
/// <remarks>
/// Parameter options:
/// - "Invert" or "!" - Inverts the boolean value
/// - "Hidden" - Uses Visibility.Hidden instead of Visibility.Collapsed for false values
/// - "Invert,Hidden" or "!,Hidden" - Combines both options
/// </remarks>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return Visibility.Collapsed;
        }

        // Parse parameter
        string paramStr = parameter?.ToString() ?? string.Empty;
        bool invert = paramStr.Contains("Invert", StringComparison.OrdinalIgnoreCase) || 
                      paramStr.Contains("!");
        bool useHidden = paramStr.Contains("Hidden", StringComparison.OrdinalIgnoreCase);

        // Apply inversion if needed
        if (invert)
        {
            boolValue = !boolValue;
        }

        // Return appropriate visibility
        if (boolValue)
        {
            return Visibility.Visible;
        }
        else
        {
            return useHidden ? Visibility.Hidden : Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility)
        {
            return false;
        }

        // Parse parameter
        string paramStr = parameter?.ToString() ?? string.Empty;
        bool invert = paramStr.Contains("Invert", StringComparison.OrdinalIgnoreCase) || 
                      paramStr.Contains("!");

        // Convert visibility to bool
        bool result = visibility == Visibility.Visible;

        // Apply inversion if needed
        if (invert)
        {
            result = !result;
        }

        return result;
    }
}
