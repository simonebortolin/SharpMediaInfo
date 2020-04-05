using GetMediaInfoGUI.Configuration;
using GetMediaInfoGUI.Utils;
using System.IO;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.History
{
    /// <summary>
    /// The history helper
    /// </summary>
    public class HistoryHelper
    {
        private static FixedSizeQueue<HistoryItem> history = new FixedSizeQueue<HistoryItem>(ConfigHelper.GetConfig().HistorySize);

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <returns>The history</returns>
        public static FixedSizeQueue<HistoryItem> GetHistory()
        {
            return history;
        }

        /// <summary>
        /// Loads the history from the file system (if present).
        /// </summary>
        /// <param name="path">The file path</param>
        public static void LoadHistory()
        {
            var path = Path.Combine(FolderUtils.GetAppDataFolder(), Constants.HistoryFileName);
            if (File.Exists(path))
            {
                // Create an instance of the XmlSerializer.
                var serializer = new XmlSerializer(typeof(HistoryItemList));

                using (Stream reader = new FileStream(path, FileMode.Open))
                {
                    var historyItemList = serializer.Deserialize(reader) as HistoryItemList;
                    history.Enqueue(historyItemList.Items.ToArray());
                }
            } 
        }

        /// <summary>
        /// Saves the history to file.
        /// </summary>
        public static void SaveHistory()
        {
            var path = Path.Combine(FolderUtils.GetAppDataFolder(), Constants.HistoryFileName);

            // Create an instance of the XmlSerializer.
            var serializer = new XmlSerializer(typeof(HistoryItemList));

            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                stream.SetLength(0);
                serializer.Serialize(writer, new HistoryItemList(history.ToArray()));
            }
        }
    }
}
