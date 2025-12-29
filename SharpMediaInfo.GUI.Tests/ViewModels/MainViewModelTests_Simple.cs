using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test semplificati per MainViewModel - test di base che funzionano
/// </summary>
public class MainViewModelTests_Simple
{
    private Mock<IFileService> _fileServiceMock = null!;
    private Mock<IDialogService> _dialogServiceMock = null!;
    private Mock<IClipboardService> _clipboardServiceMock = null!;
    private Mock<IConfigurationService> _configServiceMock = null!;
    private Mock<IRecentFilesService> _recentFilesMock = null!;
    private Mock<IMousePositionService> _mousePositionServiceMock = null!;
    private Mock<IProcessService> _processServiceMock = null!;
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private Mock<IMediaInfoAnalyzer> _mediaInfoAnalyzerMock = null!;
    private AppConfiguration _configuration = null!;
    private MainViewModel _viewModel = null!;

    [Before(Test)]
    public void Setup()
    {
        _fileServiceMock = new Mock<IFileService>();
        _dialogServiceMock = new Mock<IDialogService>();
        _clipboardServiceMock = new Mock<IClipboardService>();
        _configServiceMock = new Mock<IConfigurationService>();
        _recentFilesMock = new Mock<IRecentFilesService>();
        _mousePositionServiceMock = new Mock<IMousePositionService>();
        _processServiceMock = new Mock<IProcessService>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        _configuration = new AppConfiguration();

        _viewModel = ViewModelTestHelper.CreateMainViewModel(
            _fileServiceMock,
            _dialogServiceMock,
            _clipboardServiceMock,
            _configServiceMock,
            _recentFilesMock,
            _mousePositionServiceMock,
            _processServiceMock,
            _serviceProviderMock,
            _mediaInfoAnalyzerMock,
            _configuration);
    }

    [Test]
    public async Task Constructor_InitializesProperties()
    {
        // Assert
        await Assert.That(_viewModel).IsNotNull();
        await Assert.That(_viewModel.Tabs).IsNotNull();
        await Assert.That(_viewModel.Configuration).IsNotNull();
        await Assert.That(_viewModel.StatusMessage).IsEqualTo("Ready.");
    }

    [Test]
    public async Task Tabs_InitialState_IsEmpty()
    {
        // Assert
        await Assert.That(_viewModel.Tabs.Count).IsEqualTo(0);
    }

    [Test]
    public async Task SelectedTab_InitialState_IsNull()
    {
        // Assert
        await Assert.That(_viewModel.SelectedTab).IsNull();
    }

    [Test]
    public async Task Configuration_IsSharedInstance()
    {
        // Assert
        await Assert.That(_viewModel.Configuration).IsEqualTo(_configuration);
    }

    [Test]
    public async Task RecentFiles_InitialState_IsNotNull()
    {
        // Assert
        await Assert.That(_viewModel.RecentFiles).IsNotNull();
    }
}
