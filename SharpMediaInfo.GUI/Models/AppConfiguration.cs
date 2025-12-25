using CommunityToolkit.Mvvm.ComponentModel;

namespace SharpMediaInfo.GUI.Models;

/// <summary>
/// Application configuration model
/// </summary>
public partial class AppConfiguration : ObservableObject
{
    [ObservableProperty]
    private bool _showCover = true;

    [ObservableProperty]
    private bool _disableSimpleReport = false;

    [ObservableProperty]
    private bool _disableCompleteReport = false;

    [ObservableProperty]
    private bool _disableTreeReport = false;

    [ObservableProperty]
    private bool _showMilliseconds = true;

    [ObservableProperty]
    private bool _removeJustification = false;

    [ObservableProperty]
    private bool _autoSaveReport = false;

    [ObservableProperty]
    private string _reportFontFamily = "Cascadia Mono";

    [ObservableProperty]
    private double _reportFontSize = 12.0;

    [ObservableProperty]
    private bool _reportBoldStyle = false;

    [ObservableProperty]
    private bool _reportItalicStyle = false;

    [ObservableProperty]
    private string _theme = "Auto";

    [ObservableProperty]
    private AppTheme _appTheme = AppTheme.Auto;

    [ObservableProperty]
    private System.DateTime _lastUpdateCheck = System.DateTime.MinValue;

    [ObservableProperty]
    private int _maxRecentFiles = 10;
}
