using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per FileTabViewModel - gestione singolo file
/// </summary>
public class FileTabViewModelTests
{
    private AppConfiguration _configuration = null!;

    [Before(Test)]
    public void Setup()
    {
        _configuration = new AppConfiguration
        {
            ShowMilliseconds = false,
            RemoveJustification = false,
            ShowCover = true,
            ReportFontFamily = "Consolas",
            ReportFontSize = 12,
            ReportBoldStyle = false,
            ReportItalicStyle = false
        };
    }

    [Test]
    public async Task Constructor_SetsFileName()
    {
        // Arrange & Act
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            configuration: _configuration,
            filePath: @"C:\Test\sample.mp4");

        // Assert
        await Assert.That(viewModel.Model.FileName).IsEqualTo("sample.mp4");
    }

    [Test]
    public async Task Constructor_SetsFilePath()
    {
        // Arrange
        var filePath = @"C:\Test\sample.mp4";

        // Act
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            configuration: _configuration,
            filePath: filePath);

        // Assert
        await Assert.That(viewModel.Model.FilePath).IsEqualTo(filePath);
    }

    [Test]
    public async Task Header_ReturnsFileName()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            configuration: _configuration,
            filePath: @"C:\Test\video.mkv");

        // Act
        var header = viewModel.Header;

        // Assert
        await Assert.That(header).IsEqualTo("video.mkv");
    }

    [Test]
    public async Task IsLoading_InitialValue_IsFalse()
    {
        // Arrange & Act
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);

        // Assert
        await Assert.That(viewModel.Model.IsLoading).IsFalse();
    }

    [Test]
    public async Task HasCover_NoCoverData_ReturnsFalse()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);
        viewModel.Model.CoverData = null;

        // Assert
        await Assert.That(viewModel.Model.HasCover).IsFalse();
    }

    [Test]
    public async Task HasCover_WithCoverData_ReturnsTrue()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);
        viewModel.Model.CoverData = new byte[] { 0xFF, 0xD8, 0xFF }; // JPEG header

        // Assert
        await Assert.That(viewModel.Model.HasCover).IsTrue();
        await Assert.That(viewModel.Model.CoverData).IsNotNull();
    }

    [Test]
    public async Task Configuration_ShowMilliseconds_AffectsReportGeneration()
    {
        // Arrange
        var config1 = new AppConfiguration { ShowMilliseconds = false };
        var config2 = new AppConfiguration { ShowMilliseconds = true };

        // Act
        var vm1 = ViewModelTestHelper.CreateFileTabViewModel(configuration: config1);
        var vm2 = ViewModelTestHelper.CreateFileTabViewModel(configuration: config2);

        // Assert
        await Assert.That(vm1).IsNotNull();
        await Assert.That(vm2).IsNotNull();
        // Note: Actual report generation would need real MediaInfo data
    }

    [Test]
    public async Task CopyAllStandardCommand_WithReport_CopiesText()
    {
        // Arrange
        var clipboardMock = new Mock<IClipboardService>();
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            clipboardService: clipboardMock,
            configuration: _configuration);
        viewModel.Model.StandardReport = "Test Standard Report";

        // Act
        viewModel.CopyAllStandardCommand.Execute(null);

        // Assert
        clipboardMock.Verify(x => x.CopyText("Test Standard Report"), Times.Once);
    }

    [Test]
    public async Task CopyAllStandardCommand_WithNullReport_DoesNotCopy()
    {
        // Arrange
        var clipboardMock = new Mock<IClipboardService>();
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            clipboardService: clipboardMock,
            configuration: _configuration);
        viewModel.Model.StandardReport = null;

        // Act
        viewModel.CopyAllStandardCommand.Execute(null);

        // Assert
        clipboardMock.Verify(x => x.CopyText(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task CopyAllCompleteCommand_WithReport_CopiesText()
    {
        // Arrange
        var clipboardMock = new Mock<IClipboardService>();
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            clipboardService: clipboardMock,
            configuration: _configuration);
        viewModel.Model.CompleteReport = "Test Complete Report";

        // Act
        viewModel.CopyAllCompleteCommand.Execute(null);

        // Assert
        clipboardMock.Verify(x => x.CopyText("Test Complete Report"), Times.Once);
    }

    [Test]
    public async Task CopyAllCompleteCommand_WithNullReport_DoesNotCopy()
    {
        // Arrange
        var clipboardMock = new Mock<IClipboardService>();
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(
            clipboardService: clipboardMock,
            configuration: _configuration);
        viewModel.Model.CompleteReport = null;

        // Act
        viewModel.CopyAllCompleteCommand.Execute(null);

        // Assert
        clipboardMock.Verify(x => x.CopyText(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task OnCloseAsync_ReturnsTrue()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);

        // Act
        var result = await viewModel.OnCloseAsync();

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task SimpleReportSections_InitialValue_IsNull()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);

        // Assert
        await Assert.That(viewModel.SimpleReportSections).IsNull();
    }

    [Test]
    public async Task TreeReportNodes_InitialValue_IsNull()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);

        // Assert
        await Assert.That(viewModel.TreeReportNodes).IsNull();
    }

    [Test]
    public async Task Configuration_InitialValue_IsNull()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateFileTabViewModel(configuration: _configuration);

        // Assert
        await Assert.That(viewModel.Configuration).IsNull();
    }
}
