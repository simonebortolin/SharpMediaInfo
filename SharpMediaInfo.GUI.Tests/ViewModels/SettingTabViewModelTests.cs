using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per SettingTabViewModel - gestione impostazioni
/// </summary>
public class SettingTabViewModelTests
{
    private AppConfiguration _configuration = null!;
    private List<string> _fontFamilies = null!;

    [Before(Test)]
    public void Setup()
    {
        _configuration = new AppConfiguration();
        _fontFamilies = new List<string> { "Arial", "Consolas", "Courier New", "Times New Roman" };
    }

    [Test]
    public async Task Constructor_InitializesConfiguration()
    {
        // Act
        var viewModel = ViewModelTestHelper.CreateSettingsTabViewModel(configuration: _configuration);

        // Assert
        await Assert.That(viewModel.Configuration).IsNotNull();
        await Assert.That(viewModel.Configuration).IsEqualTo(_configuration);
    }

    [Test]
    public async Task Header_ReturnsSettings()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateSettingsTabViewModel(configuration: _configuration);

        // Assert
        await Assert.That(viewModel.Header).IsEqualTo("Settings");
    }

    [Test]
    public async Task Configuration_PropertyChanges_UpdateConfiguration()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateSettingsTabViewModel(configuration: _configuration);

        // Act
        viewModel.Configuration.ShowMilliseconds = true;
        viewModel.Configuration.RemoveJustification = true;
        viewModel.Configuration.ShowCover = false;

        // Assert
        await Assert.That(_configuration.ShowMilliseconds).IsTrue();
        await Assert.That(_configuration.RemoveJustification).IsTrue();
        await Assert.That(_configuration.ShowCover).IsFalse();
    }

    [Test]
    public async Task Configuration_ThemeChange_UpdatesTheme()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateSettingsTabViewModel(configuration: _configuration);

        // Act
        viewModel.Configuration.AppTheme = AppTheme.Dark;

        // Assert
        await Assert.That(_configuration.AppTheme).IsEqualTo(AppTheme.Dark);
    }

    [Test]
    public async Task Configuration_FontSizeChange_UpdatesFontSize()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateSettingsTabViewModel(configuration: _configuration);

        // Act
        viewModel.Configuration.ReportFontSize = 16;

        // Assert
        await Assert.That(_configuration.ReportFontSize).IsEqualTo(16);
    }

    [Test]
    public async Task SaveConfigurationCommand_SavesConfiguration()
    {
        // Arrange
        var configServiceMock = new Mock<IConfigurationService>();
        var viewModel = ViewModelTestHelper.CreateSettingsTabViewModel(
            configService: configServiceMock,
            configuration: _configuration);

        // Act
        await viewModel.SaveConfigurationCommand.ExecuteAsync(null);

        // Assert
        configServiceMock.Verify(
            x => x.SaveConfigurationAsync(_configuration),
            Times.Once);
    }

}
