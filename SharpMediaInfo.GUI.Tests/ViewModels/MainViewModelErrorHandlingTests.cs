using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per la gestione degli errori in MainViewModel
/// </summary>
public class MainViewModelErrorHandlingTests
{
    private Mock<IFileService> _fileServiceMock = null!;
    private Mock<IDialogService> _dialogServiceMock = null!;
    private Mock<IMediaInfoAnalyzer> _mediaInfoAnalyzerMock = null!;
    private MainViewModel _viewModel = null!;

    [Before(Test)]
    public void Setup()
    {
        _fileServiceMock = new Mock<IFileService>();
        _dialogServiceMock = new Mock<IDialogService>();
        _mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();

        _viewModel = ViewModelTestHelper.CreateMainViewModel(
            fileService: _fileServiceMock,
            dialogService: _dialogServiceMock,
            mediaInfoAnalyzer: _mediaInfoAnalyzerMock);
    }

    [Test]
    public async Task AddFilesFromDropAsync_AnalyzerThrowsFileNotFoundException_ShowsErrorAndRemovesTab()
    {
        // Arrange
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

        _mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new FileNotFoundException("File not found"));

        var files = new[] { "test.mp4" };

        // Act
        await _viewModel.AddFilesFromDropAsync(files);

        // Assert
        await Assert.That(_viewModel.Tabs.Count).IsEqualTo(0);
        await Assert.That(_viewModel.StatusMessage).IsEqualTo("Error.");

        _dialogServiceMock.Verify(
            x => x.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task AddFilesFromDropAsync_AnalyzerThrowsArgumentException_ShowsErrorAndRemovesTab()
    {
        // Arrange
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

        _mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new ArgumentException("Invalid argument"));

        var files = new[] { "invalid.mp4" };

        // Act
        await _viewModel.AddFilesFromDropAsync(files);

        // Assert
        await Assert.That(_viewModel.Tabs.Count).IsEqualTo(0);
        await Assert.That(_viewModel.StatusMessage).IsEqualTo("Error.");

        _dialogServiceMock.Verify(
            x => x.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task AddFilesFromDropAsync_AnalyzerThrowsIOException_ShowsErrorAndRemovesTab()
    {
        // Arrange
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

        _mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new IOException("Disk error"));

        var files = new[] { "corrupted.mp4" };

        // Act
        await _viewModel.AddFilesFromDropAsync(files);

        // Assert
        await Assert.That(_viewModel.Tabs.Count).IsEqualTo(0);
        await Assert.That(_viewModel.StatusMessage).IsEqualTo("Error.");

        _dialogServiceMock.Verify(
            x => x.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task AddFilesFromDropAsync_MultipleFiles_OneFailsOthersSucceed()
    {
        // Arrange
        _fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

        // First file succeeds, second fails, third succeeds
        _mediaInfoAnalyzerMock
            .SetupSequence(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((IMediaInfo?)null)
            .ThrowsAsync(new FileNotFoundException("File not found"))
            .ReturnsAsync((IMediaInfo?)null);

        var files = new[] { "valid1.mp4", "invalid.mp4", "valid2.mp4" };

        // Act
        await _viewModel.AddFilesFromDropAsync(files);

        // Assert
        // Should have 2 tabs (the valid ones)
        await Assert.That(_viewModel.Tabs.Count).IsEqualTo(2);

        // Should show error for the failed one
        _dialogServiceMock.Verify(
            x => x.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

}
