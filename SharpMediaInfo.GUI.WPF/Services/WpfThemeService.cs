using Microsoft.Win32;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System;
using System.Windows;

namespace SharpMediaInfo.GUI.WPF.Services;

/// <summary>
/// WPF implementation of theme service using modern .NET APIs
/// </summary>
public class WpfThemeService : IThemeService
{
    private AppTheme _currentTheme = AppTheme.Auto;

    public AppTheme CurrentTheme => _currentTheme;

    public void SetTheme(AppTheme theme)
    {
        _currentTheme = theme;
        
        // Determine actual theme to apply
        var themeToApply = theme == AppTheme.Auto ? GetSystemTheme() : theme;
        
        // Apply theme to application resources
        var themeUri = themeToApply switch
        {
            AppTheme.Dark => new Uri("pack://application:,,,/SharpMediaInfo.GUI.WPF;component/Themes/Dark.xaml"),
            _ => new Uri("pack://application:,,,/SharpMediaInfo.GUI.WPF;component/Themes/Light.xaml")
        };

        // Remove old theme dictionaries
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        for (int i = dictionaries.Count - 1; i >= 0; i--)
        {
            var dict = dictionaries[i];
            if (dict.Source != null && 
                (dict.Source.ToString().Contains("Light.xaml") || 
                 dict.Source.ToString().Contains("Dark.xaml")))
            {
                dictionaries.RemoveAt(i);
            }
        }

        // Add new theme dictionary
        dictionaries.Add(new ResourceDictionary { Source = themeUri });
    }

    public AppTheme GetSystemTheme()
    {
        try
        {
            // Use Windows Registry to detect system theme preference
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            
            if (value is int intValue)
            {
                return intValue == 0 ? AppTheme.Dark : AppTheme.Light;
            }
        }
        catch
        {
            // If detection fails, default to Light
        }

        return AppTheme.Light;
    }
}