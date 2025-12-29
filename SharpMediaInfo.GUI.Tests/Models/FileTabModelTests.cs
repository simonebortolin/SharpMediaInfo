using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.Tests.Models;

/// <summary>
/// Test per FileTabModel - modello dati delle tab dei file
/// </summary>
public class FileTabModelTests
{
    [Test]
    public async Task Constructor_InitializesWithDefaultValues()
    {
        // Act
        var model = new FileTabModel();

        // Assert
        await Assert.That(model.FilePath).IsEqualTo(string.Empty);
        await Assert.That(model.FileName).IsEqualTo(string.Empty);
        await Assert.That(model.IsLoading).IsFalse();
        await Assert.That(model.IsReady).IsFalse();
        await Assert.That(model.MediaInfo).IsNull();
        await Assert.That(model.StandardReport).IsNull();
        await Assert.That(model.CompleteReport).IsNull();
        await Assert.That(model.CoverData).IsNull();
        await Assert.That(model.HasCover).IsFalse();
    }

    [Test]
    public async Task FilePath_WhenSet_UpdatesFileName()
    {
        // Arrange
        var model = new FileTabModel();
        var fullPath = @"C:\Videos\test.mp4";

        // Act
        model.FilePath = fullPath;

        // Assert
        await Assert.That(model.FilePath).IsEqualTo(fullPath);
        await Assert.That(model.FileName).IsEqualTo("test.mp4");
    }

    [Test]
    public async Task FilePath_WithDifferentPaths_ExtractsCorrectFileName()
    {
        // Arrange
        var model = new FileTabModel();

        // Act & Assert - Windows path
        model.FilePath = @"C:\Folder\SubFolder\video.mkv";
        await Assert.That(model.FileName).IsEqualTo("video.mkv");

        // Act & Assert - Unix path
        model.FilePath = "/home/user/videos/movie.avi";
        await Assert.That(model.FileName).IsEqualTo("movie.avi");

        // Act & Assert - No extension
        model.FilePath = @"C:\test";
        await Assert.That(model.FileName).IsEqualTo("test");
    }

    [Test]
    public async Task IsLoading_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var model = new FileTabModel();
        var propertyChanged = false;

        model.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(FileTabModel.IsLoading))
                propertyChanged = true;
        };

        // Act
        model.IsLoading = true;

        // Assert
        await Assert.That(propertyChanged).IsTrue();
        await Assert.That(model.IsLoading).IsTrue();
    }

    [Test]
    public async Task IsReady_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var model = new FileTabModel();
        var propertyChanged = false;

        model.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(FileTabModel.IsReady))
                propertyChanged = true;
        };

        // Act
        model.IsReady = true;

        // Assert
        await Assert.That(propertyChanged).IsTrue();
        await Assert.That(model.IsReady).IsTrue();
    }

    [Test]
    public async Task CoverData_WhenNull_HasCoverReturnsFalse()
    {
        // Arrange
        var model = new FileTabModel();

        // Act
        model.CoverData = null;

        // Assert
        await Assert.That(model.HasCover).IsFalse();
    }

    [Test]
    public async Task CoverData_WhenEmpty_HasCoverReturnsFalse()
    {
        // Arrange
        var model = new FileTabModel();

        // Act
        model.CoverData = Array.Empty<byte>();

        // Assert
        await Assert.That(model.HasCover).IsFalse();
    }

    [Test]
    public async Task CoverData_WhenSet_HasCoverReturnsTrue()
    {
        // Arrange
        var model = new FileTabModel();
        var coverData = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        model.CoverData = coverData;

        // Assert
        await Assert.That(model.HasCover).IsTrue();
        await Assert.That(model.CoverData).IsEqualTo(coverData);
    }

    [Test]
    public async Task CoverData_PropertyChanged_NotifiesHasCover()
    {
        // Arrange
        var model = new FileTabModel();
        var hasCoverNotified = false;

        model.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(FileTabModel.HasCover))
                hasCoverNotified = true;
        };

        // Act
        model.CoverData = new byte[] { 1, 2, 3 };

        // Assert
        await Assert.That(hasCoverNotified).IsTrue();
    }

    [Test]
    public async Task MediaInfo_CanBeSetAndRetrieved()
    {
        // Arrange
        var model = new FileTabModel();
        var mediaInfoMock = new Mock<IMediaInfo>();

        // Act
        model.MediaInfo = mediaInfoMock.Object;

        // Assert
        await Assert.That(model.MediaInfo).IsEqualTo(mediaInfoMock.Object);
    }

    [Test]
    public async Task StandardReport_CanBeSetAndRetrieved()
    {
        // Arrange
        var model = new FileTabModel();
        var report = "Standard Report Content";

        // Act
        model.StandardReport = report;

        // Assert
        await Assert.That(model.StandardReport).IsEqualTo(report);
    }

    [Test]
    public async Task CompleteReport_CanBeSetAndRetrieved()
    {
        // Arrange
        var model = new FileTabModel();
        var report = "Complete Report Content";

        // Act
        model.CompleteReport = report;

        // Assert
        await Assert.That(model.CompleteReport).IsEqualTo(report);
    }
}
