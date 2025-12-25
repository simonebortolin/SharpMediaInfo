using SharpMediaInfo.GUI.Models.Report;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SharpMediaInfo.GUI.WPF.Controls;

/// <summary>
/// WPF UserControl that renders ReportSection[] as formatted text
/// </summary>
public partial class SimpleReportViewer : UserControl
{
    public static readonly DependencyProperty SectionsProperty =
        DependencyProperty.Register(
            nameof(Sections),
            typeof(IEnumerable<ReportSection>),
            typeof(SimpleReportViewer),
            new PropertyMetadata(null, OnSectionsChanged));

    public static readonly DependencyProperty ConfigurationProperty =
        DependencyProperty.Register(
            nameof(Configuration),
            typeof(SharpMediaInfo.GUI.Models.AppConfiguration),
            typeof(SimpleReportViewer),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CoverDataProperty =
        DependencyProperty.Register(
            nameof(CoverData),
            typeof(byte[]),
            typeof(SimpleReportViewer),
            new PropertyMetadata(null));

    public static readonly DependencyProperty HasCoverProperty =
        DependencyProperty.Register(
            nameof(HasCover),
            typeof(bool),
            typeof(SimpleReportViewer),
            new PropertyMetadata(false));

    public static readonly DependencyProperty SaveCoverCommandProperty =
        DependencyProperty.Register(
            nameof(SaveCoverCommand),
            typeof(ICommand),
            typeof(SimpleReportViewer),
            new PropertyMetadata(null));

    public IEnumerable<ReportSection>? Sections
    {
        get => (IEnumerable<ReportSection>?)GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    public SharpMediaInfo.GUI.Models.AppConfiguration? Configuration
    {
        get => (SharpMediaInfo.GUI.Models.AppConfiguration?)GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public byte[]? CoverData
    {
        get => (byte[]?)GetValue(CoverDataProperty);
        set => SetValue(CoverDataProperty, value);
    }

    public bool HasCover
    {
        get => (bool)GetValue(HasCoverProperty);
        set => SetValue(HasCoverProperty, value);
    }

    public ICommand? SaveCoverCommand
    {
        get => (ICommand?)GetValue(SaveCoverCommandProperty);
        set => SetValue(SaveCoverCommandProperty, value);
    }

    public SimpleReportViewer()
    {
        InitializeComponent();
    }

    private static void OnSectionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimpleReportViewer viewer)
        {
            viewer.SectionsContainer.ItemsSource = e.NewValue as IEnumerable<ReportSection>;
        }
    }
}

/// <summary>
/// Converts indent level to left margin
/// </summary>
public class IndentToMarginConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length > 0 && values[0] is int indentLevel)
        {
            return new Thickness(indentLevel * 20, 2, 0, 0);
        }
        return new Thickness(0, 2, 0, 0);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
