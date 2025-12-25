using CommunityToolkit.Mvvm.ComponentModel;

namespace SharpMediaInfo.GUI.Models.Report;

/// <summary>
/// Represents a single item in a report (key-value pair)
/// </summary>
public partial class ReportItem : ObservableObject
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public ReportItemType Type { get; init; } = ReportItemType.Normal;
    public int IndentLevel { get; init; } = 0;

    public ReportItem() { }

    public ReportItem(string label, string value, ReportItemType type = ReportItemType.Normal, int indentLevel = 0)
    {
        Label = label;
        Value = value;
        Type = type;
        IndentLevel = indentLevel;
    }
}

/// <summary>
/// Type of report item for styling purposes
/// </summary>
public enum ReportItemType
{
    Normal,
    Header,
    Subheader,
    Separator
}
