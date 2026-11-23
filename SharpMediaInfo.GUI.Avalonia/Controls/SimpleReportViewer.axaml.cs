using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using System.Collections.Generic;
using System.Windows.Input;

namespace SharpMediaInfo.GUI.Avalonia.Controls;

public partial class SimpleReportViewer : UserControl
{
    [GeneratedStyledProperty]
    public partial IEnumerable<ReportSection>? Sections { get; set; }

    [GeneratedStyledProperty]
    public partial AppConfiguration? Configuration { get; set; }

    [GeneratedStyledProperty]
    public partial byte[]? CoverData { get; set; }

    [GeneratedStyledProperty]
    public partial bool HasCover { get; set; }

    [GeneratedStyledProperty]
    public partial ICommand? SaveCoverCommand { get; set; }

    public SimpleReportViewer()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
