using SharpMediaInfo.GUI.Models;

namespace SharpMediaInfo.GUI.Tests.Models;

/// <summary>
/// Test per AppConfiguration - configurazione applicazione
/// </summary>
public class AppConfigurationTests
{
    [Test]
    public async Task Constructor_InitializesWithDefaultValues()
    {
        // Act
        var config = new AppConfiguration();

        // Assert
        await Assert.That(config.ShowCover).IsTrue();
        await Assert.That(config.DisableSimpleReport).IsFalse();
        await Assert.That(config.DisableCompleteReport).IsFalse();
        await Assert.That(config.DisableTreeReport).IsFalse();
        await Assert.That(config.ShowMilliseconds).IsTrue();
        await Assert.That(config.RemoveJustification).IsFalse();
        await Assert.That(config.AutoSaveReport).IsFalse();
        await Assert.That(config.ReportFontFamily).IsEqualTo("Cascadia Mono");
        await Assert.That(config.ReportFontSize).IsEqualTo(12.0);
        await Assert.That(config.ReportBoldStyle).IsFalse();
        await Assert.That(config.ReportItalicStyle).IsFalse();
        await Assert.That(config.Theme).IsEqualTo("Auto");
        await Assert.That(config.AppTheme).IsEqualTo(AppTheme.Auto);
    }

    [Test]
    public async Task ShowCover_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var config = new AppConfiguration();
        var propertyChanged = false;

        config.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AppConfiguration.ShowCover))
                propertyChanged = true;
        };

        // Act
        config.ShowCover = false;

        // Assert
        await Assert.That(propertyChanged).IsTrue();
        await Assert.That(config.ShowCover).IsFalse();
    }

    [Test]
    public async Task ShowMilliseconds_CanBeToggled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.ShowMilliseconds = false;

        // Assert
        await Assert.That(config.ShowMilliseconds).IsFalse();
    }

    [Test]
    public async Task ReportFontFamily_CanBeChanged()
    {
        // Arrange
        var config = new AppConfiguration();
        var newFont = "Consolas";

        // Act
        config.ReportFontFamily = newFont;

        // Assert
        await Assert.That(config.ReportFontFamily).IsEqualTo(newFont);
    }

    [Test]
    public async Task ReportFontSize_CanBeChanged()
    {
        // Arrange
        var config = new AppConfiguration();
        var newSize = 14.0;

        // Act
        config.ReportFontSize = newSize;

        // Assert
        await Assert.That(config.ReportFontSize).IsEqualTo(newSize);
    }

    [Test]
    public async Task ReportBoldStyle_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.ReportBoldStyle = true;

        // Assert
        await Assert.That(config.ReportBoldStyle).IsTrue();
    }

    [Test]
    public async Task ReportItalicStyle_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.ReportItalicStyle = true;

        // Assert
        await Assert.That(config.ReportItalicStyle).IsTrue();
    }

    [Test]
    public async Task AppTheme_CanBeSetToLight()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.AppTheme = AppTheme.Light;

        // Assert
        await Assert.That(config.AppTheme).IsEqualTo(AppTheme.Light);
    }

    [Test]
    public async Task AppTheme_CanBeSetToDark()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.AppTheme = AppTheme.Dark;

        // Assert
        await Assert.That(config.AppTheme).IsEqualTo(AppTheme.Dark);
    }

    [Test]
    public async Task DisableSimpleReport_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.DisableSimpleReport = true;

        // Assert
        await Assert.That(config.DisableSimpleReport).IsTrue();
    }

    [Test]
    public async Task DisableCompleteReport_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.DisableCompleteReport = true;

        // Assert
        await Assert.That(config.DisableCompleteReport).IsTrue();
    }

    [Test]
    public async Task DisableTreeReport_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.DisableTreeReport = true;

        // Assert
        await Assert.That(config.DisableTreeReport).IsTrue();
    }

    [Test]
    public async Task AutoSaveReport_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.AutoSaveReport = true;

        // Assert
        await Assert.That(config.AutoSaveReport).IsTrue();
    }

    [Test]
    public async Task RemoveJustification_CanBeEnabled()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.RemoveJustification = true;

        // Assert
        await Assert.That(config.RemoveJustification).IsTrue();
    }

    [Test]
    public async Task MaxRecentFiles_CanBeSet()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        config.MaxRecentFiles = 20;

        // Assert
        await Assert.That(config.MaxRecentFiles).IsEqualTo(20);
    }
}
