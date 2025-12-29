using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.Tests.Models;

/// <summary>
/// Test per FileDialogOptions - opzioni dialog di selezione file
/// </summary>
public class FileDialogOptionsTests
{
    [Test]
    public async Task Constructor_InitializesWithDefaultValues()
    {
        // Act
        var options = new FileDialogOptions();

        // Assert
        await Assert.That(options.Title).IsEqualTo("Select a file");
        await Assert.That(options.FileType).IsEqualTo(FileType.All);
        await Assert.That(options.DefaultFileName).IsEqualTo("file");
        await Assert.That(options.DefaultExtension).IsEqualTo("*.*");
        await Assert.That(options.AllowMultipleSelection).IsFalse();
    }

    [Test]
    public async Task Title_CanBeSetAndRetrieved()
    {
        // Arrange
        var options = new FileDialogOptions();
        var customTitle = "Choose a media file";

        // Act
        options.Title = customTitle;

        // Assert
        await Assert.That(options.Title).IsEqualTo(customTitle);
    }

    [Test]
    public async Task FileType_CanBeSetToMedia()
    {
        // Arrange
        var options = new FileDialogOptions();

        // Act
        options.FileType = FileType.Media;

        // Assert
        await Assert.That(options.FileType).IsEqualTo(FileType.Media);
    }

    [Test]
    public async Task FileType_CanBeSetToAll()
    {
        // Arrange
        var options = new FileDialogOptions();

        // Act
        options.FileType = FileType.All;

        // Assert
        await Assert.That(options.FileType).IsEqualTo(FileType.All);
    }

    [Test]
    public async Task DefaultFileName_CanBeCustomized()
    {
        // Arrange
        var options = new FileDialogOptions();
        var customName = "report";

        // Act
        options.DefaultFileName = customName;

        // Assert
        await Assert.That(options.DefaultFileName).IsEqualTo(customName);
    }

    [Test]
    public async Task DefaultExtension_CanBeCustomized()
    {
        // Arrange
        var options = new FileDialogOptions();
        var customExtension = "*.txt";

        // Act
        options.DefaultExtension = customExtension;

        // Assert
        await Assert.That(options.DefaultExtension).IsEqualTo(customExtension);
    }

    [Test]
    public async Task AllowMultipleSelection_CanBeEnabled()
    {
        // Arrange
        var options = new FileDialogOptions();

        // Act
        options.AllowMultipleSelection = true;

        // Assert
        await Assert.That(options.AllowMultipleSelection).IsTrue();
    }

    [Test]
    public async Task ObjectInitializer_CanSetAllProperties()
    {
        // Act
        var options = new FileDialogOptions
        {
            Title = "Open Video",
            FileType = FileType.Media,
            DefaultFileName = "movie",
            DefaultExtension = "*.mp4",
            AllowMultipleSelection = true
        };

        // Assert
        await Assert.That(options.Title).IsEqualTo("Open Video");
        await Assert.That(options.FileType).IsEqualTo(FileType.Media);
        await Assert.That(options.DefaultFileName).IsEqualTo("movie");
        await Assert.That(options.DefaultExtension).IsEqualTo("*.mp4");
        await Assert.That(options.AllowMultipleSelection).IsTrue();
    }
}
