namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per il costruttore di MainViewModel - validazione parametri
/// </summary>
public class MainViewModelConstructorTests
{
    [Test]
    public async Task Constructor_NullFileService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            null!,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("fileService");
    }

    [Test]
    public async Task Constructor_NullDialogService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            null!,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("dialogService");
    }

    [Test]
    public async Task Constructor_NullClipboardService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            null!,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("clipboardService");
    }

    [Test]
    public async Task Constructor_NullConfigurationService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            null!,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("configurationService");
    }

    [Test]
    public async Task Constructor_NullRecentFilesService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            null!,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("recentFilesService");
    }

    [Test]
    public async Task Constructor_NullMousePositionService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            null!,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("mousePositionService");
    }

    [Test]
    public async Task Constructor_NullProcessService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            null!,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("processService");
    }

    [Test]
    public async Task Constructor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            null!,
            new Mock<IMediaInfoAnalyzer>().Object,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("serviceProvider");
    }

    [Test]
    public async Task Constructor_NullMediaInfoAnalyzer_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            new Mock<IRecentFilesService>().Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            null!,
            new AppConfiguration()))
            .Throws<ArgumentNullException>()
            .WithParameterName("mediaInfoAnalyzer");
    }

    [Test]
    public async Task Constructor_NullConfiguration_ThrowsArgumentNullException()
    {
        // Arrange
        var recentFilesMock = new Mock<IRecentFilesService>();
        recentFilesMock
            .Setup(x => x.LoadAsync(It.IsAny<int>()))
            .ReturnsAsync(new System.Collections.ObjectModel.ObservableCollection<string>());

        // Act & Assert
        await Assert.That(() => new MainViewModel(
            new Mock<IFileService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IConfigurationService>().Object,
            recentFilesMock.Object,
            new Mock<IMousePositionService>().Object,
            new Mock<IProcessService>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<IMediaInfoAnalyzer>().Object,
            null!))
            .Throws<ArgumentNullException>()
            .WithParameterName("configuration");
    }
}
