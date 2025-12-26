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
            new PropertyMetadata(null, OnNodesChanged));

    public static readonly DependencyProperty ConfigurationProperty =
        DependencyProperty.Register(
            nameof(Configuration),
            typeof(Models.AppConfiguration),
            typeof(TreeReportViewer),
            new PropertyMetadata(null));

    public IEnumerable<TreeNode>? Nodes
    {
        get => (IEnumerable<TreeNode>?)GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public Models.AppConfiguration? Configuration
    {
        get => (Models.AppConfiguration?)GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public TreeReportViewer()
    {
        InitializeComponent();
    }

    private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TreeReportViewer viewer)
        {
            viewer.TreeContainer.ItemsSource = e.NewValue as IEnumerable<TreeNode>;
        }
    }
}
