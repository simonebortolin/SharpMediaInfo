using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using System.Collections.Generic;

namespace SharpMediaInfo.GUI.Avalonia.Controls;

public partial class TreeReportViewer : UserControl
{
    public static readonly StyledProperty<IEnumerable<TreeNode>?> NodesProperty =
        AvaloniaProperty.Register<TreeReportViewer, IEnumerable<TreeNode>?>(nameof(Nodes));

    public static readonly StyledProperty<AppConfiguration?> ConfigurationProperty =
        AvaloniaProperty.Register<TreeReportViewer, AppConfiguration?>(nameof(Configuration));

    public IEnumerable<TreeNode>? Nodes
    {
        get => GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    public AppConfiguration? Configuration
    {
        get => GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public TreeReportViewer()
    {
        InitializeComponent();
    }
}
