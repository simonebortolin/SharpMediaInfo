using System.Diagnostics;
using System.IO;

namespace GetMediaInfoGUI.Util
{
    public class FileUtils
    {
        /// <summary>
        /// Retrieves the version of a specific assembly.
        /// </summary>
        /// <param name="path">The path of the assembly</param>
        /// <returns>The assembly version, if it's present; otherwise empty string</returns>
        public static string GetAssemblyVersion(string path)
        {
            if (File.Exists(path))
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);

                if (!string.IsNullOrEmpty(fileVersionInfo.FileVersion))
                {
                    return fileVersionInfo.FileVersion;
                }
            }

            return string.Empty;
        }
    }
}
