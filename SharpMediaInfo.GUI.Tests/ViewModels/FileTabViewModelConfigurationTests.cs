using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per la gestione della configurazione in FileTabViewModel
/// </summary>
public class FileTabViewModelConfigurationTests
{
    [Test]
    public async Task InitializeAsync_WithShowCoverTrue_AnalyzesWithCoverData()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((IMediaInfo?)null);

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "test.mp4");

        var config = new AppConfiguration { ShowCover = true };

        // Act
        await viewModel.InitializeAsync(config);

        // Assert
        mediaInfoAnalyzerMock.Verify(
            x => x.AnalyzeFileAsync("test.mp4", true),
            Times.Once);
    }

    [Test]
    public async Task InitializeAsync_WithShowCoverFalse_AnalyzesWithoutCoverData()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((IMediaInfo?)null);

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "test.mp4");

        var config = new AppConfiguration { ShowCover = false };

        // Act
        await viewModel.InitializeAsync(config);

        // Assert
        mediaInfoAnalyzerMock.Verify(
            x => x.AnalyzeFileAsync("test.mp4", false),
            Times.Once);
    }

    [Test]
    public async Task RefreshAsync_CallsAnalyzerWithCurrentConfiguration()
    {
        // Arrange
        var mediaInfoAnalyzerMock = new Mock<IMediaInfoAnalyzer>();
        mediaInfoAnalyzerMock
            .Setup(x => x.AnalyzeFileAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((IMediaInfo?)null);

        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            mediaInfoAnalyzer: mediaInfoAnalyzerMock,
            filePath: "test.mp4");

        var config = new AppConfiguration { ShowCover = true };
        await viewModel.InitializeAsync(config);

        // Act
        await viewModel.RefreshAsync(config);

        // Assert
        mediaInfoAnalyzerMock.Verify(
            x => x.AnalyzeFileAsync("test.mp4", true),
            Times.Exactly(2)); // Once for init, once for refresh
    }
}
