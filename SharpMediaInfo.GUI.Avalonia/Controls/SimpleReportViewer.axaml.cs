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
    public static readonly StyledProperty<IEnumerable<ReportSection>?> SectionsProperty =
        AvaloniaProperty.Register<SimpleReportViewer, IEnumerable<ReportSection>?>(nameof(Sections));

    public static readonly StyledProperty<AppConfiguration?> ConfigurationProperty =
        AvaloniaProperty.Register<SimpleReportViewer, AppConfiguration?>(nameof(Configuration));

    public static readonly StyledProperty<byte[]?> CoverDataProperty =
        AvaloniaProperty.Register<SimpleReportViewer, byte[]?>(nameof(CoverData));

    public static readonly StyledProperty<bool> HasCoverProperty =
        AvaloniaProperty.Register<SimpleReportViewer, bool>(nameof(HasCover));

    public static readonly StyledProperty<ICommand?> SaveCoverCommandProperty =
        AvaloniaProperty.Register<SimpleReportViewer, ICommand?>(nameof(SaveCoverCommand));

    public IEnumerable<ReportSection>? Sections
    {
        get => GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    public AppConfiguration? Configuration
    {
        get => GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public byte[]? CoverData
    {
        get => GetValue(CoverDataProperty);
        set => SetValue(CoverDataProperty, value);
    }

    public bool HasCover
    {
        get => GetValue(HasCoverProperty);
        set => SetValue(HasCoverProperty, value);
    }

    public ICommand? SaveCoverCommand
    {
        get => GetValue(SaveCoverCommandProperty);
        set => SetValue(SaveCoverCommandProperty, value);
    }

    public SimpleReportViewer()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
