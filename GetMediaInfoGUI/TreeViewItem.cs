using System.Collections.Generic;

namespace GetMediaInfoGUI
{
    public class TreeViewItem
    {
        public TreeViewItem()
        {
            Children = new List<TreeViewItem>();
        }

        public string Header { get; set; }

        public IList<TreeViewItem> Children { get; set; }
    }
}
