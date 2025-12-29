using SharpMediaInfo.GUI.Models.Report;

namespace SharpMediaInfo.GUI.Tests.Models.Report;

/// <summary>
/// Test per ReportSection - costruttori e proprietà
/// </summary>
public class ReportSectionTests
{
    [Test]
    public async Task Constructor_Default_InitializesEmptyProperties()
    {
        // Act
        var section = new ReportSection();

        // Assert
        await Assert.That(section.Title).IsEqualTo(string.Empty);
        await Assert.That(section.Items).IsNotNull();
        await Assert.That(section.Items.Count).IsEqualTo(0);
        await Assert.That(section.IsVisible).IsTrue();
    }

    [Test]
    public async Task Constructor_WithTitle_SetsTitle()
    {
        // Arrange
        var title = "Video";

        // Act
        var section = new ReportSection(title);

        // Assert
        await Assert.That(section.Title).IsEqualTo(title);
        await Assert.That(section.Items).IsNotNull();
        await Assert.That(section.Items.Count).IsEqualTo(0);
        await Assert.That(section.IsVisible).IsTrue();
    }

    [Test]
    public async Task Constructor_WithTitleAndItems_SetsBoth()
    {
        // Arrange
        var title = "Audio";
        var items = new List<ReportItem>
        {
            new ReportItem("Codec", "AAC"),
            new ReportItem("Bitrate", "128 kbps")
        };

        // Act
        var section = new ReportSection(title, items);

        // Assert
        await Assert.That(section.Title).IsEqualTo(title);
        await Assert.That(section.Items).IsNotNull();
        await Assert.That(section.Items.Count).IsEqualTo(2);
        await Assert.That(section.Items[0].Label).IsEqualTo("Codec");
        await Assert.That(section.Items[0].Value).IsEqualTo("AAC");
        await Assert.That(section.Items[1].Label).IsEqualTo("Bitrate");
        await Assert.That(section.Items[1].Value).IsEqualTo("128 kbps");
        await Assert.That(section.IsVisible).IsTrue();
    }

    [Test]
    public async Task IsVisible_SetFalse_UpdatesProperty()
    {
        // Arrange
        var section = new ReportSection("General");

        // Act
        section.IsVisible = false;

        // Assert
        await Assert.That(section.IsVisible).IsFalse();
    }

    [Test]
    public async Task IsVisible_SetTrue_UpdatesProperty()
    {
        // Arrange
        var section = new ReportSection("Text") { IsVisible = false };

        // Act
        section.IsVisible = true;

        // Assert
        await Assert.That(section.IsVisible).IsTrue();
    }
}
