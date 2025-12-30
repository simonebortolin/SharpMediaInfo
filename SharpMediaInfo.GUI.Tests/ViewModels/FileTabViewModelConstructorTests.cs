namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per il costruttore di FileTabViewModel - validazione parametri
/// </summary>
public class FileTabViewModelConstructorTests
{
    [Test]
    public async Task Constructor_NullModel_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new FileTabViewModel(
            null!,
            new Mock<IFileService>().Object,
            new Mock<IClipboardService>().Object,
            new Mock<IMediaInfoAnalyzer>().Object))
            .Throws<ArgumentNullException>()
            .WithParameterName("model");
    }

    [Test]
    public async Task Constructor_NullFileService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new FileTabViewModel(
            new FileTabModel(),
            null!,
            new Mock<IClipboardService>().Object,
            new Mock<IMediaInfoAnalyzer>().Object))
            .Throws<ArgumentNullException>()
            .WithParameterName("fileService");
    }

    [Test]
    public async Task Constructor_NullClipboardService_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new FileTabViewModel(
            new FileTabModel(),
            new Mock<IFileService>().Object,
            null!,
            new Mock<IMediaInfoAnalyzer>().Object))
            .Throws<ArgumentNullException>()
            .WithParameterName("clipboardService");
    }

    [Test]
    public async Task Constructor_NullMediaInfoAnalyzer_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.That(() => new FileTabViewModel(
            new FileTabModel(),
            new Mock<IFileService>().Object,
            new Mock<IClipboardService>().Object,
            null!))
            .Throws<ArgumentNullException>()
            .WithParameterName("mediaInfoAnalyzer");
    }
}
