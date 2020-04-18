using System.Windows;

namespace GetMediaInfoGUI.Theme
{
    /// <summary>
    /// Windows theme detector
    /// </summary>
    public static class WindowsThemeDetector
    {
        private const string RegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryKey = "AppsUseLightTheme";

        /// <summary>
        /// Retrieves the current configured windows theme.
        /// </summary>
        /// <returns>The current theme</returns>
        public static AppTheme GetCurrentTheme()
        {
            try
            {
                return GetThemeFromRegistry();
            }
            catch
            {
                try
                {
                    return GetThemeFromAPI();
                }
                catch { }
            }

            return AppTheme.Light;
        }

        /// <summary>
        /// Gets the theme from the Windows UWP API.
        /// </summary>
        /// <returns>The current theme</returns>
        private static AppTheme GetThemeFromAPI()
        {
            return AppTheme.Light;
            /*var accessibilitySettings = new Windows.UI.ViewManagement.AccessibilitySettings();
                
            if (accessibilitySettings.HighContrast)
            {
                return AppTheme.HighContrast;
            }

            var settings = new Windows.UI.ViewManagement.UISettings();
            var foreground = settings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Foreground);
            var background = settings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background);

            if (foreground == Windows.UI.Colors.White && background == Windows.UI.Colors.Black)
            {
                return AppTheme.Dark;
            }

            return AppTheme.Light;*/
        }

        /// <summary>
        /// Gets the theme from the registry.
        /// </summary>
        /// <returns>The current theme</returns>
        private static AppTheme GetThemeFromRegistry()
        {
            if (SystemParameters.HighContrast)
            {
                return AppTheme.HighContrast;
            }

            var value = Microsoft.Win32.Registry.GetValue(RegistryPath, RegistryKey, "1").ToString();
            if (!string.IsNullOrEmpty(value) && value == "0")
            {
                return AppTheme.Dark;
            }

            return AppTheme.Light;
        }
    }
}