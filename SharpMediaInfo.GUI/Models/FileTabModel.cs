using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SharpMediaInfo.GUI.ViewModels;

namespace SharpMediaInfo.GUI.Models;

/// <summary>
/// Represents a file tab model with media information
/// </summary>
public partial class FileTabModel : ObservableObject
{
    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isReady;

    [ObservableProperty]
    private SharpMediaInfo.MediaInfo? _mediaInfo;

    [ObservableProperty]
    private string? _standardReport;

    [ObservableProperty]
    private string? _completeReport;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCover))]
    private byte[]? _coverData;

    public bool HasCover => CoverData != null && CoverData.Length > 0;

    partial void OnFilePathChanged(string value)
    {
        FileName = System.IO.Path.GetFileName(value);
    }
}
