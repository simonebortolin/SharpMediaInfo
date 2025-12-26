using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpMediaInfo.GUI.Converters;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Models.Report;
using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.ViewModels;

/// <summary>
/// View model for a file tab
/// </summary>
public partial class FileTabViewModel : ObservableObject, ITabViewModel
{
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;
    private readonly IClipboardService _clipboardService;

    [ObservableProperty]
    private FileTabModel _model;

    [ObservableProperty]
    private List<ReportSection>? _simpleReportSections;

    [ObservableProperty]
    private List<TreeNode>? _treeReportNodes;

    [ObservableProperty]
    private AppConfiguration? _configuration;

    public string Header => Model.FileName;

    private bool _configurationSubscribed;


    public FileTabViewModel(FileTabModel model, IFileService fileService, IDialogService dialogService, IClipboardService clipboardService)
    {
        _fileService = fileService ?? throw new System.ArgumentNullException(nameof(fileService));
        _dialogService = dialogService ?? throw new System.ArgumentNullException(nameof(dialogService));
        _clipboardService = clipboardService ?? throw new System.ArgumentNullException(nameof(clipboardService));
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public async Task InitializeAsync(AppConfiguration configuration)
    {
        Configuration = configuration;

        if (!_configurationSubscribed && Configuration != null)
        {
            Configuration.PropertyChanged += Configuration_PropertyChanged;
            _configurationSubscribed = true;
        }

        Model.IsLoading = true;
        Model.IsReady = false;

        try
        {
            await Task.Run(() =>
            {
#if DEBUG
                var sw = Stopwatch.StartNew();
#endif
                var builder = SharpMediaInfo.MediaInfoBuilder.FromFile(Model.FilePath)
                    .WithCoverData(configuration.ShowCover)
                    .WithReports();

                Model.MediaInfo = builder.Build();

                if (Model.MediaInfo != null)
                {
                    ApplyReports(configuration);
                }

#if DEBUG
                sw.Stop();
                Debug.WriteLine($"File: {Model.FilePath} - Analysis time: {sw.ElapsedMilliseconds}ms");
#endif
            });

            Model.IsReady = true;

            // Auto-save report if enabled
            if (configuration.AutoSaveReport && Model.MediaInfo != null)
            {
                await AutoSaveReportAsync();
            }
        }
        finally
        {
            Model.IsLoading = false;
        }
    }

    public async Task RefreshAsync(AppConfiguration configuration)
    {
        await InitializeAsync(configuration);
    }

    private async void Configuration_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Model.MediaInfo == null || Configuration == null)
            return;

        switch (e.PropertyName)
        {
            case nameof(AppConfiguration.ShowCover):
                // Reload cover data when ShowCover changes
                await ReloadCoverAsync();
                break;
            case nameof(AppConfiguration.RemoveJustification):
            case nameof(AppConfiguration.ShowMilliseconds):
            case nameof(AppConfiguration.DisableSimpleReport):
            case nameof(AppConfiguration.DisableTreeReport):
            case nameof(AppConfiguration.ReportBoldStyle):
            case nameof(AppConfiguration.ReportItalicStyle):
            case nameof(AppConfiguration.ReportFontFamily):
            case nameof(AppConfiguration.ReportFontSize):
                ApplyReports(Configuration);
                break;
        }
    }

    private async Task ReloadCoverAsync()
    {
        if (Model.MediaInfo == null || Configuration == null)
            return;

        await Task.Run(() =>
        {
            var builder = SharpMediaInfo.MediaInfoBuilder.FromFile(Model.FilePath)
                .WithCoverData(Configuration.ShowCover)
                .WithReports();

            Model.MediaInfo = builder.Build();
        });

        ApplyReports(Configuration);
    }

    private void ApplyReports(AppConfiguration configuration)
    {
        if (Model.MediaInfo == null)
            return;

        Model.StandardReport = configuration.RemoveJustification
            ? NormalizeJustification(Model.MediaInfo.InfoStandard)
            : Model.MediaInfo.InfoStandard;

        Model.CompleteReport = configuration.RemoveJustification
            ? NormalizeJustification(Model.MediaInfo.InfoComplete)
            : Model.MediaInfo.InfoComplete;

        if (!configuration.DisableSimpleReport)
        {
            var reportConverter = new TrackToReportConverter(configuration);
            SimpleReportSections = reportConverter.Convert(Model.MediaInfo);
        }
        else
        {
            SimpleReportSections = null;
        }

        if (!configuration.DisableTreeReport)
        {
            var treeConverter = new TrackToTreeConverter(configuration);
            TreeReportNodes = treeConverter.Convert(Model.MediaInfo);
        }
        else
        {
            TreeReportNodes = null;
        }

#if DEBUG
        Debug.WriteLine($"ShowCover: {configuration.ShowCover}");
        Debug.WriteLine($"General.Count: {Model.MediaInfo.General.Count}");
        if (Model.MediaInfo.General.Count > 0)
        {
            Debug.WriteLine($"Cover property: {Model.MediaInfo.General[0].Cover}");
            Debug.WriteLine($"CoverData.Count: {Model.MediaInfo.General[0].CoverData.Count}");
        }
#endif

        if (configuration.ShowCover &&
            Model.MediaInfo.General.Count > 0 &&
            Model.MediaInfo.General[0].Cover.GetValueOrDefault() &&
            Model.MediaInfo.General[0].CoverData.Count > 0)
        {
            Model.CoverData = Model.MediaInfo.General[0].CoverData[0];
#if DEBUG
            Debug.WriteLine($"Cover loaded! Size: {Model.CoverData.Length} bytes");
#endif
        }
        else
        {
            Model.CoverData = null;
#if DEBUG
            Debug.WriteLine("No cover data available");
#endif
        }
    }

    [RelayCommand]
    private void CopyAllStandard()
    {
        if (Model.StandardReport != null)
        {
            _clipboardService.CopyText(Model.StandardReport);
        }
    }

    [RelayCommand]
    private void CopyAllComplete()
    {
        if (Model.CompleteReport != null)
        {
            _clipboardService.CopyText(Model.CompleteReport);
        }
    }

    public Task<bool> OnCloseAsync()
    {
        return Task.FromResult(true);
    }

    private string NormalizeJustification(string? report)
    {
        if (string.IsNullOrEmpty(report)) return report ?? string.Empty;
        var normalized = Regex.Replace(report, " {2,}", " ");
        normalized = normalized.Replace(" : ", ": ");
        return normalized;
    }

    /// <summary>
    /// Automatically saves the standard report to disk
    /// </summary>
    private async Task AutoSaveReportAsync()
    {
        if (Model.StandardReport == null || Model.MediaInfo?.General.Count == 0)
            return;

        try
        {
            var completeName = Model.MediaInfo?.General[0].CompleteName;
            if (string.IsNullOrEmpty(completeName))
                return;

            var reportPath = Path.ChangeExtension(completeName, ".txt");
            await _fileService.WriteTextFileAsync(Model.StandardReport, reportPath);

#if DEBUG
            Debug.WriteLine($"Auto-saved report to: {reportPath}");
#endif
        }
        catch (System.Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"Failed to auto-save report: {ex.Message}");
#endif
            // Silently fail for auto-save - don't interrupt user workflow
        }
    }
}
