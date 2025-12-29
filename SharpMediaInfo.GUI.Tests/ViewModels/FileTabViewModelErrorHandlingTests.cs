using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per la gestione degli errori in FileTabViewModel
/// </summary>
public class FileTabViewModelErrorHandlingTests
{
    [Test]
    public async Task InitializeAsync_AnalyzerThrowsFileNotFoundException_ThrowsException()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new FileNotFoundException("File not found"));

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "nonexistent.mp4");

        var config = new AppConfiguration { ShowCover = true };

        // Act & Assert
        await Assert.That(async () => await viewModel.InitializeAsync(config))
            .Throws<FileNotFoundException>();
    }

    [Test]
    public async Task InitializeAsync_AnalyzerThrowsArgumentException_ThrowsException()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new ArgumentException("Invalid file path"));

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "invalid.mp4");

        var config = new AppConfiguration { ShowCover = true };

        // Act & Assert
        await Assert.That(async () => await viewModel.InitializeAsync(config))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task InitializeAsync_AnalyzerThrowsIOException_ThrowsException()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new IOException("Disk error"));

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "corrupted.mp4");

        var config = new AppConfiguration { ShowCover = true };

        // Act & Assert
        await Assert.That(async () => await viewModel.InitializeAsync(config))
            .Throws<IOException>();
    }

    [Test]
    public async Task RefreshAsync_AnalyzerThrowsException_ThrowsException()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();

        // First call succeeds, second fails
        mediaInfoAnalyzerMock
            .SetupSequence(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((IMediaInfo?)null)
            .ThrowsAsync(new IOException("Access denied"));

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "test.mp4");

        var config = new AppConfiguration { ShowCover = true };
        await viewModel.InitializeAsync(config);

        // Act & Assert
        await Assert.That(async () => await viewModel.RefreshAsync(config))
            .Throws<IOException>();
    }
}
