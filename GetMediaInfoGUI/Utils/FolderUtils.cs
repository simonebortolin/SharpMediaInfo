using System;
using System.IO;
using System.Windows;

namespace GetMediaInfoGUI.Utils
{
    /// <summary>
    /// Folder utilities.
    /// </summary>
    public class FolderUtils
    {
        /// <summary>
        /// Gets the app data folder where to save the config files, creates it if it does not exists.
        /// </summary>
        /// <returns>The path to the appdata folder.</returns>
        public static string GetAppDataFolder()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.Current.MainWindow.GetType()?.Assembly.GetName()?.Name);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }
    }
}
