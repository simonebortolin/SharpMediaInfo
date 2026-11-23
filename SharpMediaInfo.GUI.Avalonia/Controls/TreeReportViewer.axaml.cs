using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using System.Collections.Generic;

namespace SharpMediaInfo.GUI.Avalonia.Controls;

public partial class TreeReportViewer : UserControl
{
    [GeneratedStyledProperty]
    public partial IEnumerable<TreeNode>? Nodes { get; set; }

    [GeneratedStyledProperty]
    public partial AppConfiguration? Configuration { get; set; }

    public TreeReportViewer()
    {
        InitializeComponent();
    }
}
