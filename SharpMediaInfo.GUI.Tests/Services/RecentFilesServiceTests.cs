namespace SharpMediaInfo.GUI.Tests.Services;

/// <summary>
/// Test per RecentFilesService - gestione file recenti
/// </summary>
public class RecentFilesServiceTests
{
    [Test]
    public async Task LoadAsync_ReturnsEmptyCollection_WhenNoFileExists()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var service = new RecentFilesService(fileServiceMock.Object);

        // Act
        var recentFiles = await service.LoadAsync(10);

        // Assert
        await Assert.That(recentFiles).IsNotNull();
        await Assert.That(recentFiles.Count).IsEqualTo(0);
    }

    [Test]
    public async Task AddAsync_AddsFileToCollection()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var service = new RecentFilesService(fileServiceMock.Object);
        var recentFiles = await service.LoadAsync(10);

        // Act
        await service.AddAsync("C:\\test.mp4", recentFiles, 10);

        // Assert
        await Assert.That(recentFiles.Count).IsEqualTo(1);
        await Assert.That(recentFiles[0]).IsEqualTo("C:\\test.mp4");
    }

    [Test]
    public async Task AddAsync_MovesExistingFileToTop()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var service = new RecentFilesService(fileServiceMock.Object);
        var recentFiles = await service.LoadAsync(10);

        await service.AddAsync("C:\\file1.mp4", recentFiles, 10);
        await service.AddAsync("C:\\file2.mp4", recentFiles, 10);

        // Act
        await service.AddAsync("C:\\file1.mp4", recentFiles, 10);

        // Assert
        await Assert.That(recentFiles.Count).IsEqualTo(2);
        await Assert.That(recentFiles[0]).IsEqualTo("C:\\file1.mp4");
        await Assert.That(recentFiles[1]).IsEqualTo("C:\\file2.mp4");
    }

    [Test]
    public async Task AddAsync_EnforcesMaxLimit()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var service = new RecentFilesService(fileServiceMock.Object);
        var recentFiles = await service.LoadAsync(3);

        // Act
        await service.AddAsync("C:\\file1.mp4", recentFiles, 3);
        await service.AddAsync("C:\\file2.mp4", recentFiles, 3);
        await service.AddAsync("C:\\file3.mp4", recentFiles, 3);
        await service.AddAsync("C:\\file4.mp4", recentFiles, 3);

        // Assert
        await Assert.That(recentFiles.Count).IsEqualTo(3);
        await Assert.That(recentFiles[0]).IsEqualTo("C:\\file4.mp4");
        await Assert.That(recentFiles.Contains("C:\\file1.mp4")).IsFalse();
    }

    [Test]
    public async Task ClearAsync_RemovesAllFiles()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var service = new RecentFilesService(fileServiceMock.Object);
        var recentFiles = await service.LoadAsync(10);

        await service.AddAsync("C:\\file1.mp4", recentFiles, 10);
        await service.AddAsync("C:\\file2.mp4", recentFiles, 10);

        // Act
        await service.ClearAsync(recentFiles);

        // Assert
        await Assert.That(recentFiles.Count).IsEqualTo(0);
    }
}
