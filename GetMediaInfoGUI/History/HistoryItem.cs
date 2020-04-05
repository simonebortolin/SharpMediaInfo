using System;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.History
{
    /// <summary>
    /// An history item.
    /// </summary>
    public class HistoryItem
    {
        /// <summary>
        /// The path of the file.
        /// </summary>
        [XmlElement("file")]
        public string Path { get; set; }
        /// <summary>
        /// The timestamp.
        /// </summary>
        [XmlElement(DataType = "dateTime")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
       
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HistoryItem() { }

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="path">The file path</param>
        /// <param name="timestamp">The timestamp</param>
        public HistoryItem(string path, DateTime timestamp)
        {
            this.Path = path;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Secondary constructor (timestamp default is now).
        /// </summary>
        /// <param name="path">The file path</param>
        public HistoryItem(string path) :  this(path, DateTime.Now)
        {

        }
    }
}
