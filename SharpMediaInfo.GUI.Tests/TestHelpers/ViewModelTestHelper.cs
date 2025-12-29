namespace SharpMediaInfo.GUI.Tests.TestHelpers;

/// <summary>
/// Helper per creare ViewModels con mock services per i test
/// </summary>
public static class ViewModelTestHelper
{
    public static MainViewModel CreateMainViewModel(
        Mock<IFileService>? fileService = null,
        Mock<IDialogService>? dialogService = null,
        Mock<IClipboardService>? clipboardService = null,
        Mock<IConfigurationService>? configService = null,
        Mock<IRecentFilesService>? recentFilesService = null,
        Mock<IMousePositionService>? mousePositionService = null,
        Mock<IProcessService>? processService = null,
        Mock<IServiceProvider>? serviceProvider = null,
        Mock<IMediaInfoAnalyzer>? mediaInfoAnalyzer = null,
        AppConfiguration? configuration = null)
    {
        fileService ??= new Mock<IFileService>();
        dialogService ??= new Mock<IDialogService>();
        clipboardService ??= new Mock<IClipboardService>();
        configService ??= new Mock<IConfigurationService>();
        recentFilesService ??= new Mock<IRecentFilesService>();
        mousePositionService ??= new Mock<IMousePositionService>();
        processService ??= new Mock<IProcessService>();
        serviceProvider ??= new Mock<IServiceProvider>();
        mediaInfoAnalyzer ??= new Mock<IMediaInfoAnalyzer>();
        configuration ??= new AppConfiguration();

        // Setup default behaviors
        recentFilesService
            .Setup(x => x.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(new System.Collections.ObjectModel.ObservableCollection<string>());

        return new MainViewModel(
            fileService.Object,
            dialogService.Object,
            clipboardService.Object,
            configService.Object,
            recentFilesService.Object,
            mousePositionService.Object,
            processService.Object,
            serviceProvider.Object,
            mediaInfoAnalyzer.Object,
            configuration);
    }

    public static FileTabViewModel CreateFileTabViewModel(
        Mock<IFileService>? fileService = null,
        Mock<IClipboardService>? clipboardService = null,
        Mock<IMediaInfoAnalyzer>? mediaInfoAnalyzer = null,
        AppConfiguration? configuration = null,
        string filePath = "test.mp4")
    {
        fileService ??= new Mock<IFileService>();
        clipboardService ??= new Mock<IClipboardService>();
        mediaInfoAnalyzer ??= new Mock<IMediaInfoAnalyzer>();
        configuration ??= new AppConfiguration();

        var model = new FileTabModel
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath)
        };

        var viewModel = new FileTabViewModel(
            model,
            fileService.Object,
            clipboardService.Object,
            mediaInfoAnalyzer.Object);

        return viewModel;
    }

    public static SettingTabViewModel CreateSettingsTabViewModel(
        Mock<IConfigurationService>? configService = null,
        Mock<IContextMenuService>? contextMenuService = null,
        Mock<IDialogService>? dialogService = null,
        AppConfiguration? configuration = null)
    {
        configService ??= new Mock<IConfigurationService>();
        contextMenuService ??= new Mock<IContextMenuService>();
        dialogService ??= new Mock<IDialogService>();
        configuration ??= new AppConfiguration();

        // Setup default behaviors
        contextMenuService
            .Setup(x => x.IsInstalledAsync())
            .ReturnsAsync(false);

        return new SettingTabViewModel(
            configService.Object,
            configuration,
            contextMenuService.Object,
            dialogService.Object);
    }

    public static AboutTabViewModel CreateAboutTabViewModel()
    {
        return new AboutTabViewModel();
    }
}
