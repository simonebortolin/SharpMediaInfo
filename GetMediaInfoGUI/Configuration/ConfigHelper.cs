using GetMediaInfoGUI.Utils;
using System.IO;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.Configuration
{
    /// <summary>
    /// The configuration provider.
    /// </summary>
    public class ConfigHelper
    {
        private static Configuration configuration = new Configuration();

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <returns>The configuration</returns>
        public static Configuration GetConfig()
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
                // Create an instance of the XmlSerializer.
                var serializer = new XmlSerializer(typeof(Configuration));

                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    configuration = serializer.Deserialize(reader) as Configuration;
                }
            } else
            {
                throw new FileNotFoundException("Unable to load configuration, fhe file was not found");
            } 
        }

        /// <summary>
        /// Saves the configuration to file.
        /// </summary>
        public static void SaveConfig()
        {
            var path = Path.Combine(FolderUtils.GetAppDataFolder(), Constants.ConfigurationFileName);

            // Create an instance of the XmlSerializer.
            var serializer = new XmlSerializer(typeof(Configuration));

            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                stream.SetLength(0);
                serializer.Serialize(writer, configuration);
            }
        }
    }
}
