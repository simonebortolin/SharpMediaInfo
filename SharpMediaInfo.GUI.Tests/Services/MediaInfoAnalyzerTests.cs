namespace SharpMediaInfo.GUI.Tests.Services;

/// <summary>
/// Test per MediaInfoAnalyzer - validazione e comportamento
/// </summary>
public class MediaInfoAnalyzerTests
{
    [Test]
    public async Task AnalyzeFile_NullFilePath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();

        // Act & Assert
        await Assert.That(() => analyzer.AnalyzeFile(null!, false))
            .Throws<ArgumentException>()
            .WithParameterName("filePath");
    }

    [Test]
    public async Task AnalyzeFile_EmptyFilePath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();

        // Act & Assert
        await Assert.That(() => analyzer.AnalyzeFile("", false))
            .Throws<ArgumentException>()
            .WithParameterName("filePath");
    }

    [Test]
    public async Task AnalyzeFile_WhitespaceFilePath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();

        // Act & Assert
        await Assert.That(() => analyzer.AnalyzeFile("   ", false))
            .Throws<ArgumentException>()
            .WithParameterName("filePath");
    }

    [Test]
    public async Task AnalyzeFile_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();
        var nonExistentPath = "C:\\nonexistent\\file.mp4";

        // Act & Assert
        await Assert.That(() => analyzer.AnalyzeFile(nonExistentPath, false))
            .Throws<FileNotFoundException>();
    }

    [Test]
    public async Task AnalyzeFileAsync_NullFilePath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();

        // Act & Assert
        await Assert.That(async () => await analyzer.AnalyzeFileAsync(null!, false))
            .Throws<ArgumentException>()
            .WithParameterName("filePath");
    }

    [Test]
    public async Task AnalyzeFileAsync_EmptyFilePath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();

        // Act & Assert
        await Assert.That(async () => await analyzer.AnalyzeFileAsync("", false))
            .Throws<ArgumentException>()
            .WithParameterName("filePath");
    }

    [Test]
    public async Task AnalyzeFileAsync_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var analyzer = new MediaInfoAnalyzer();
        var nonExistentPath = "C:\\nonexistent\\file.mp4";

        // Act & Assert
        await Assert.That(async () => await analyzer.AnalyzeFileAsync(nonExistentPath, false))
            .Throws<FileNotFoundException>();
    }
}
