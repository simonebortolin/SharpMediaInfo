using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SharpMediaInfo.GUI.Models.Report;

/// <summary>
/// Represents a node in a tree structure (platform-agnostic)
/// </summary>
public partial class TreeNode : ObservableObject
{
    public string Header { get; init; } = string.Empty;
    public string? Value { get; init; }
    public List<TreeNode> Children { get; init; } = new();

    [ObservableProperty]
    private bool _isExpanded = false;

    public TreeNodeType Type { get; init; } = TreeNodeType.Normal;

    public TreeNode() { }

    public TreeNode(string header, string? value = null)
    {
        Header = header;
        Value = value;
    }

    public TreeNode(string header, List<TreeNode> children)
    {
        Header = header;
        Children = children;
    }
}

/// <summary>
/// Type of tree node for styling
/// </summary>
public enum TreeNodeType
{
    Normal,
    Section,
    Track,
    Property
}
