using SharpMediaInfo.GUI.Tests.TestHelpers;

namespace SharpMediaInfo.GUI.Tests.ViewModels;

/// <summary>
/// Test per AboutTabViewModel
/// </summary>
public class AboutTabViewModelTests
{
    private Mock<IProcessService> _processServiceMock = null!;

    [Before(Test)]
    public void Setup()
    {
        _processServiceMock = new Mock<IProcessService>();
    }

    [Test]
    public async Task Header_ReturnsAbout()
    {
        // Arrange
        var viewModel = ViewModelTestHelper.CreateAboutTabViewModel();

        // Assert
        await Assert.That(viewModel.Header).IsEqualTo("About");
    }

    [Test]
    public async Task Constructor_CreatesViewModel()
    {
        // Arrange & Act
        var viewModel = ViewModelTestHelper.CreateAboutTabViewModel();

        // Assert
        await Assert.That(viewModel).IsNotNull();
    }
}
