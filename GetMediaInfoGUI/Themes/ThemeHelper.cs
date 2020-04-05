using GetMediaInfoGUI.Configuration;
using GetMediaInfoGUI.Theme;
using System;
using System.Windows;

namespace GetMediaInfoGUI.Themes
{
    /// <summary>
    /// The theme helper.
    /// </summary>
    public class ThemeHelper
    {
        private static readonly ResourceDictionary DarkTheme = new ResourceDictionary() { Source = new Uri(@"\Themes\Resources\Dark.xaml", UriKind.RelativeOrAbsolute) };
        private static readonly ResourceDictionary LightTheme = new ResourceDictionary() { Source = new Uri(@"\Themes\Resources\Light.xaml", UriKind.RelativeOrAbsolute) };
        private static readonly ResourceDictionary HighContrastTheme = new ResourceDictionary() { Source = new Uri(@"\Themes\Resources\HighContrast.xaml", UriKind.RelativeOrAbsolute) };

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        /// <returns>The current application theme</returns>
        public static AppTheme GetTheme()
        {
            return ConfigHelper.GetConfig().Theme;
        }

        /// <summary>
        /// Sets the application theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns>True, if the operation is successful; otherwise false</returns>
        public static bool SetTheme(AppTheme theme)
        {
            try 
            {
                App.Current.Resources.MergedDictionaries.Clear();

                //find the current theme if the automatic selection is specified
                var newTheme = theme == AppTheme.Auto ? WindowsThemeDetector.GetCurrentTheme() : theme;

                switch (newTheme)
                {
                    case AppTheme.Light:
                        App.Current.Resources.MergedDictionaries.Add(LightTheme);
                        break;
                    case AppTheme.Dark:
                        App.Current.Resources.MergedDictionaries.Add(DarkTheme);
                        break;
                    case AppTheme.HighContrast:
                        App.Current.Resources.MergedDictionaries.Add(HighContrastTheme);
                        break;
                }

                ConfigHelper.GetConfig().Theme = theme;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parses the application theme from the input value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The parsed app theme.</returns>
        public static AppTheme ParseAppTheme(string value)
        {
            switch (value ?? "Auto")
            {
                default:
                case "Auto":
                    return AppTheme.Auto;
                case "Light":
                    return AppTheme.Light;
                case "Dark":
                    return AppTheme.Dark;
            }
        }
    }
}
