using System.Collections.Generic;
using System.Xml.Serialization;

namespace GetMediaInfoGUI.History
{
    /// <summary>
    /// Recent history item
    /// </summary>
    [XmlRoot("history")]
    public class HistoryItemList
    {
        [XmlElement("item")]
        public List<HistoryItem> Items { get; set; } = new List<HistoryItem>();

        public HistoryItemList()
        {

        }

        public HistoryItemList(ICollection<HistoryItem> items)
        {
            Items.AddRange(items);
        }
    }
}
