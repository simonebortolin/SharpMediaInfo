using System.Collections.Generic;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// TreeViewItem Class binded with XAML interface
    /// </summary>
    public class TreeViewItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItem" /> class.
        /// </summary>
        public TreeViewItem()
        {
            Children = new List<TreeViewItem>();
        }

        /// <summary>
        /// Gets or sets the header title.
        /// </summary>
        /// <value>
        /// The header title of the TreeViewItem.
        /// </value>
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the childrens list.
        /// </summary>
        /// <value>
        /// The list of all childrens items.
        /// </value>
        public IList<TreeViewItem> Children { get; set; }
    }
}
