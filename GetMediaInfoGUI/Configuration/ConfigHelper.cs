using GetMediaInfoGUI.Logging;
using GetMediaInfoGUI.Util;
using System.IO;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.Configuration
{
    /// <summary>
    /// The configuration provider.
    /// </summary>
    public class ConfigHelper
    {
        private static Config configuration = new Config();

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>The configuration</returns>
        public static Config GetConfig()
        {
            return configuration;
        }

        /// <summary>
        /// Loads the configuration from the file system (if present).
        /// </summary>
        /// <param name="path">The file path</param>
        public static void LoadConfig()
        {
            var path = Path.Combine(FolderUtils.GetAppDataFolder(), Constants.ConfigurationFileName);
            if (File.Exists(path))
            {
                var serializer = new XmlSerializer(typeof(Config));
                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    configuration = serializer.Deserialize(reader) as Config;
                }
            } else
            {
                Logger.Instance.Error(new FileNotFoundException("Configuration file not found"));
            }
        }

        /// <summary>
        /// Saves the configuration to file.
        /// </summary>
        public static void SaveConfig()
        {
            var path = Path.Combine(FolderUtils.GetAppDataFolder(), Constants.ConfigurationFileName);

            var serializer = new XmlSerializer(typeof(Config));

            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                stream.SetLength(0);
                serializer.Serialize(writer, configuration);
            }
        }
    }
}
