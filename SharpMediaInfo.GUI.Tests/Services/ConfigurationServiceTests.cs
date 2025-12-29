using System.ComponentModel;

namespace SharpMediaInfo.GUI.Tests.Services;

/// <summary>
/// Test per ConfigurationService - caricamento e salvataggio configurazione
/// </summary>
public class ConfigurationServiceTests
{
    [Test]
    public async Task LoadConfigurationAsync_WhenFileDoesNotExist_ReturnsDefaultConfiguration()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

        var service = new ConfigurationService(fileServiceMock.Object);

        // Act
        var config = await service.LoadConfigurationAsync();

        // Assert
        await Assert.That(config).IsNotNull();
        await Assert.That(config.ShowCover).IsTrue(); // Default value
    }

    [Test]
    public async Task LoadConfigurationAsync_WhenFileExists_DeserializesJsonConfiguration()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var jsonConfig = "{\"ShowCover\":false,\"ShowMilliseconds\":true,\"DisableTreeReport\":true,\"MaxRecentFiles\":5}";

        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        fileServiceMock.Setup(x => x.ReadTextFileAsync(It.IsAny<string>()))
            .ReturnsAsync(jsonConfig);

        var service = new ConfigurationService(fileServiceMock.Object);

        // Act
        var config = await service.LoadConfigurationAsync();

        // Assert
        await Assert.That(config).IsNotNull();
        await Assert.That(config.ShowCover).IsFalse();
        await Assert.That(config.ShowMilliseconds).IsTrue();
        await Assert.That(config.DisableTreeReport).IsTrue();
        await Assert.That(config.MaxRecentFiles).IsEqualTo(5);
    }

    [Test]
    public async Task LoadConfigurationAsync_WhenJsonIsInvalid_ThrowsJsonException()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var invalidJson = "{invalid json}";

        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        fileServiceMock.Setup(x => x.ReadTextFileAsync(It.IsAny<string>()))
            .ReturnsAsync(invalidJson);

        var service = new ConfigurationService(fileServiceMock.Object);

        // Act & Assert - Should throw JsonException
        await Assert.That(async () => await service.LoadConfigurationAsync())
            .Throws<System.Text.Json.JsonException>();
    }

    [Test]
    public async Task LoadConfigurationAsync_WhenJsonIsEmpty_ReturnsConfigurationWithDefaults()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();

        fileServiceMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
        fileServiceMock.Setup(x => x.ReadTextFileAsync(It.IsAny<string>()))
            .ReturnsAsync("{}");

        var service = new ConfigurationService(fileServiceMock.Object);

        // Act
        var config = await service.LoadConfigurationAsync();

        // Assert - Empty JSON deserializes to object with default property values
        await Assert.That(config).IsNotNull();
    }

    [Test]
    public async Task SaveConfigurationAsync_CallsFileServiceWithSerializedJson()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var service = new ConfigurationService(fileServiceMock.Object);
        var config = new AppConfiguration { ShowCover = false, ShowMilliseconds = true };

        // Act
        await service.SaveConfigurationAsync(config);

        // Assert
        fileServiceMock.Verify(
            x => x.WriteTextFileAsync(
                It.Is<string>(json => json.Contains("\"ShowCover\": false") && json.Contains("\"ShowMilliseconds\": true")),
                It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task Attach_WhenPropertyChanges_CallsSaveConfigurationAsync()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var service = new ConfigurationService(fileServiceMock.Object);
        var config = new AppConfiguration();

        // Act
        service.Attach(config);
        config.ShowCover = false; // Trigger PropertyChanged

        // Wait a bit for async save to complete
        await Task.Delay(100);

        // Assert
        fileServiceMock.Verify(
            x => x.WriteTextFileAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.AtLeastOnce);
    }

    [Test]
    public async Task Attach_WhenCalledTwice_OnlyAttachesOnce()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var service = new ConfigurationService(fileServiceMock.Object);
        var config = new AppConfiguration();

        // Act
        service.Attach(config);
        service.Attach(config); // Second call should be ignored
        config.ShowCover = false;

        // Wait for async save
        await Task.Delay(100);

        // Assert - Should only save once (from first attachment)
        fileServiceMock.Verify(
            x => x.WriteTextFileAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public async Task Attach_WhenConfigurationIsNull_DoesNotThrow()
    {
        // Arrange
        var fileServiceMock = new Mock<IFileService>();
        var service = new ConfigurationService(fileServiceMock.Object);

        // Act
        service.Attach(null!);

        // Assert - Just verify no exception was thrown
        await Task.CompletedTask;
    }
}
