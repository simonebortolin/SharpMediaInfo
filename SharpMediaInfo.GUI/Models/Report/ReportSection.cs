using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SharpMediaInfo.GUI.Models.Report;

/// <summary>
/// Represents a section of a report (General, Video, Audio, etc.)
/// </summary>
public partial class ReportSection : ObservableObject
{
    public string Title { get; init; } = string.Empty;
    public List<ReportItem> Items { get; init; } = new();

    [ObservableProperty]
    private bool _isVisible = true;

    public ReportSection() { }

    public ReportSection(string title)
    {
        Title = title;
    }

    public ReportSection(string title, List<ReportItem> items)
    {
        Title = title;
        Items = items;
    }
}
