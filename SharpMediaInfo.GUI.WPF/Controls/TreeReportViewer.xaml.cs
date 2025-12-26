using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SharpMediaInfo.GUI.WPF.Controls;

/// <summary>
/// WPF UserControl that renders TreeNode[] as TreeView
/// </summary>
public partial class TreeReportViewer : UserControl
{
    public static readonly DependencyProperty NodesProperty =
        DependencyProperty.Register(
            nameof(Nodes),
            typeof(IEnumerable<TreeNode>),
            typeof(TreeReportViewer),
            new PropertyMetadata(null));

    public static readonly DependencyProperty ConfigurationProperty =
        DependencyProperty.Register(
            nameof(Configuration),
            typeof(AppConfiguration),
            typeof(TreeReportViewer),
            new PropertyMetadata(null));

    public IEnumerable<TreeNode>? Nodes
    {
        get => (IEnumerable<TreeNode>?)GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public AppConfiguration? Configuration
    {
        get => (AppConfiguration?)GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public TreeReportViewer()
    {
        InitializeComponent();
    }
}
