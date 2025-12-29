using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharpMediaInfo.GUI.Models;
using SharpMediaInfo.GUI.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.ViewModels;

/// <summary>
/// Main window view model
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;
    private readonly IClipboardService _clipboardService;
    private readonly IConfigurationService _configurationService;
    private readonly IRecentFilesService _recentFilesService;
    private readonly IMousePositionService _mousePositionService;
    private readonly IProcessService _processService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediaInfoAnalyzer _mediaInfoAnalyzer;

    [ObservableProperty]
    private ObservableCollection<ITabViewModel> _tabs = new();

    [ObservableProperty]
    private ITabViewModel? _selectedTab;

    partial void OnSelectedTabChanged(ITabViewModel? value)
    {
        SaveStandardReportCommand.NotifyCanExecuteChanged();
        SaveCompleteReportCommand.NotifyCanExecuteChanged();
        SaveCoverCommand.NotifyCanExecuteChanged();
        SaveSimpleReportCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private string _statusMessage = "Ready.";

    [ObservableProperty]
    private AppConfiguration _configuration;

    [ObservableProperty]
    private ObservableCollection<string> _recentFiles = new();

    public MainViewModel(
        IFileService fileService,
        IDialogService dialogService,
        IClipboardService clipboardService,
        IConfigurationService configurationService,
        IRecentFilesService recentFilesService,
        IMousePositionService mousePositionService,
        IProcessService processService,
        IServiceProvider serviceProvider,
        IMediaInfoAnalyzer mediaInfoAnalyzer,
        AppConfiguration configuration)
    {
        _fileService = fileService ?? throw new System.ArgumentNullException(nameof(fileService));
        _dialogService = dialogService ?? throw new System.ArgumentNullException(nameof(dialogService));
        _clipboardService = clipboardService ?? throw new System.ArgumentNullException(nameof(clipboardService));
        _configurationService = configurationService ?? throw new System.ArgumentNullException(nameof(configurationService));
        _mediaInfoAnalyzer = mediaInfoAnalyzer ?? throw new System.ArgumentNullException(nameof(mediaInfoAnalyzer));
        _recentFilesService = recentFilesService ?? throw new System.ArgumentNullException(nameof(recentFilesService));
        _mousePositionService = mousePositionService ?? throw new System.ArgumentNullException(nameof(mousePositionService));
        _processService = processService ?? throw new System.ArgumentNullException(nameof(processService));
        _configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
        _serviceProvider = serviceProvider ?? throw new System.ArgumentNullException(nameof(serviceProvider));

        // Attach autosave to the shared configuration instance
        _configurationService.Attach(Configuration);
 
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        // Configuration is already loaded, just load recent files
        RecentFiles = await _recentFilesService.LoadAsync(MaxRecentFilesAllowed);
    }

    private int MaxRecentFilesAllowed => Math.Max(1, Configuration.MaxRecentFiles);

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var filePath = await _fileService.OpenFileAsync(FileType.All);
        if (!string.IsNullOrEmpty(filePath))
        {
            await AddFileTabAsync(filePath);
        }
    }

    [RelayCommand]
    private async Task OpenFilesAsync()
    {
        var filePaths = await _fileService.OpenFilesAsync(FileType.All);
        if (filePaths != null && filePaths.Length > 0)
        {
            foreach (var filePath in filePaths)
            {
                await AddFileTabAsync(filePath);
            }
        }
    }

    [RelayCommand]
    private async Task OpenFolderAsync()
    {
        var folderPath = await _fileService.OpenFolderAsync();
        if (!string.IsNullOrEmpty(folderPath) && _fileService.DirectoryExists(folderPath))
        {
            var files = _fileService.GetFilesInDirectory(folderPath);
            foreach (var file in files)
            {
                await AddFileTabAsync(file);
            }
        }
    }

    [RelayCommand]
    private void CloseAllFiles()
    {
        Tabs.Clear();
        StatusMessage = "Ready.";
    }

    [RelayCommand(CanExecute = nameof(CanSaveReport))]
    private async Task SaveStandardReportAsync()
    {
        if (SelectedTab is FileTabViewModel fileTabModel && fileTabModel?.Model.StandardReport != null)
        {
            await _fileService.SaveTextFileAsync(
                fileTabModel.Model.StandardReport, FileType.Text,
                System.IO.Path.GetFileNameWithoutExtension(fileTabModel.Model.FilePath) + ".txt",
                ".txt");
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveReport))]
    private async Task SaveCompleteReportAsync()
    {
        if (SelectedTab is FileTabViewModel fileTabModel && fileTabModel?.Model.CompleteReport != null)
        {
            await _fileService.SaveTextFileAsync(
                fileTabModel.Model.CompleteReport, FileType.Text,
                System.IO.Path.GetFileNameWithoutExtension(fileTabModel.Model.FilePath) + ".txt",
                ".txt");
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveCover))]
    private async Task SaveCoverAsync()
    {
        if (SelectedTab is FileTabViewModel fileTabModel && fileTabModel?.Model.CoverData != null)
        {
            await _fileService.SaveBinaryFileAsync(
                fileTabModel.Model.CoverData, FileType.Image,
                System.IO.Path.GetFileNameWithoutExtension(fileTabModel.Model.FilePath) + ".jpeg",
                ".jpeg");
        }
    }

    [RelayCommand]
    private async Task OpenRecentFile(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            await AddFilesFromDropAsync(new[] { filePath });
        } 
        else
        {
            RecentFiles.Remove(filePath);
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveReport))]
    private async Task SaveSimpleReportAsync()
    {
        if (SelectedTab is not FileTabViewModel fileTabModel || fileTabModel?.SimpleReportSections == null || fileTabModel.SimpleReportSections.Count == 0)
        {
            return;
        }

        var filePath = _fileService.ShowSaveFileDialog("Save Simple Report", "Text Files|*.txt|All Files|*.*", "SimpleReport.txt");

        if (!string.IsNullOrEmpty(filePath))
        {
            var reportContent = GenerateSimpleReportContent(fileTabModel);
            await File.WriteAllTextAsync(filePath, reportContent);
        }
    }

    private string GenerateSimpleReportContent(FileTabViewModel fileTabModel)
    {
        var sb = new StringBuilder();

        foreach (var section in fileTabModel.SimpleReportSections ?? [])
        {
            sb.AppendLine(section.Title);
            sb.AppendLine(new string('-', section.Title.Length));

            foreach (var item in section.Items)
            {
                sb.AppendLine($"{item.Label}: {item.Value}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private bool CanSaveReport() => SelectedTab is FileTabViewModel fileTabModel && fileTabModel.Model.IsReady;

    private bool CanSaveCover() => SelectedTab is FileTabViewModel fileTabModel && fileTabModel.Model.HasCover;

    private async Task AddFileTabAsync(string filePath)
    {
        if (!_fileService.FileExists(filePath))
        {
            await _dialogService.ShowErrorAsync($"File not found: {filePath}");
            return;
        }

        // Check if file is already open
        var existingTab = Tabs.FirstOrDefault(tab => tab is FileTabViewModel ftab && ftab.Model.FilePath.Equals(filePath, System.StringComparison.OrdinalIgnoreCase));
        if (existingTab != null)
        {
            // Select the existing tab
            SelectedTab = existingTab;
            return;
        }

        var fileTab = new FileTabViewModel(
            new FileTabModel { FilePath = filePath },
            _fileService,
            _clipboardService,
            _mediaInfoAnalyzer);
        
        Tabs.Add(fileTab);
        
        // Select the newly added tab
        SelectedTab = fileTab;

        StatusMessage = $"Analyzing {System.IO.Path.GetFileName(filePath)}...";

        try
        {
            await fileTab.InitializeAsync(Configuration);
            StatusMessage = "Ready.";
        }
        catch (System.Exception ex)
        {
            await _dialogService.ShowErrorAsync($"Error analyzing file: {ex.Message}");
            Tabs.Remove(fileTab);
            StatusMessage = "Error.";
        }

        SaveStandardReportCommand.NotifyCanExecuteChanged();
        SaveCompleteReportCommand.NotifyCanExecuteChanged();
        SaveCoverCommand.NotifyCanExecuteChanged();
        SaveSimpleReportCommand.NotifyCanExecuteChanged();

        await AddToRecentFilesAsync(filePath);
    }

    [RelayCommand]
    public async Task CloseTab(ITabViewModel fileTab)
    {
        if (fileTab == null)
            return;
        var canClose = await fileTab.OnCloseAsync();
        if (canClose)
        {
            Tabs.Remove(fileTab);
            if (Tabs.Count == 0)
            {
                StatusMessage = "Ready.";
            }
        }
    }

    public async Task AddFilesFromDropAsync(string[] files)
    {
        if (files.Length > 10)
        {
            var result = await _dialogService.ShowConfirmationAsync(
                $"Are you sure you want to open {files.Length} files?",
                "Multiple files opening confirmation");

            if (!result)
            {
                return;
            }
        }

        foreach (var file in files)
        {
            await AddFileTabAsync(file);
        }
    }

    public async Task AddToRecentFilesAsync(string filePath)
    {
        await _recentFilesService.AddAsync(filePath, RecentFiles, MaxRecentFilesAllowed);
    }

    [RelayCommand]
    private void OpenSetting()
    {
        var existingSettingsTab = Tabs.FirstOrDefault(tab => tab is SettingTabViewModel);
        if (existingSettingsTab != null)
        {
            SelectedTab = existingSettingsTab;
            return;
        }

        var settingsViewModel = _serviceProvider.GetRequiredService<SettingTabViewModel>();
        Tabs.Add(settingsViewModel);
        SelectedTab = settingsViewModel;
    }

    [RelayCommand]
    private void OpenAbout()
    {
        var existingAboutTab = Tabs.FirstOrDefault(tab => (tab is AboutTabViewModel) && (tab  is not SettingTabViewModel));
        if (existingAboutTab != null)
        {
            SelectedTab = existingAboutTab;
            return;
        }

        var aboutViewModel = _serviceProvider.GetRequiredService<AboutTabViewModel>();
        Tabs.Add(aboutViewModel);
        SelectedTab = aboutViewModel;
    }

    [RelayCommand]
    private async Task ClearRecentFilesAsync()
    {
        await _recentFilesService.ClearAsync(RecentFiles);
    }

    #region Advanced Drag & Drop

    /// <summary>
    /// Complete tab drag outside window - creates new instance or transfers to existing
    /// </summary>
    [RelayCommand]
    private async Task CompleteTabDragOutsideAsync(FileTabViewModel tab)
    {
        if (tab == null) return;

        try
        {
            // Get current mouse position for window placement
            var mousePos = _mousePositionService.GetScreenPosition();
            
            // Use ProcessService to start new instance
            var arguments = $"--window-position {mousePos.X},{mousePos.Y} \"{tab.Model.FilePath}\"";
            var success = await _processService.StartNewInstanceAsync(arguments);

            if (success)
            {
                // Remove tab from current instance only if new instance started successfully
                Tabs.Remove(tab);
            }
            else
            {
                await _dialogService.ShowErrorAsync("Failed to create new instance");
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"ERROR creating new instance: {ex.Message}");
            await _dialogService.ShowErrorAsync($"Failed to create new instance: {ex.Message}");
        }
    }

    #endregion
}
