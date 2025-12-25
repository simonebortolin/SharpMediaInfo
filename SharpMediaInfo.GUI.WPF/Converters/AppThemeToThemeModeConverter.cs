using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.WPF.Converters;

/// <summary>
/// Converts shared AppTheme enum to WPF ThemeMode.
/// </summary>
public sealed class AppThemeToThemeModeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var appTheme = value as AppTheme? ?? AppTheme.Auto;

        if (appTheme == AppTheme.Light)
            return ThemeMode.Light;
        if (appTheme == AppTheme.Dark)
            return ThemeMode.Dark;

        return ThemeMode.System;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ThemeMode mode)
        {
            if (mode == ThemeMode.Light)
                return AppTheme.Light;
            if (mode == ThemeMode.Dark)
                return AppTheme.Dark;
            if (mode == ThemeMode.System)
                return AppTheme.Auto;
        }

        return AppTheme.Auto;
    }
}
