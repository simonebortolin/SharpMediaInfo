using SharpMediaInfo.GUI.Models.Report;

namespace SharpMediaInfo.GUI.Tests.Models.Report;

/// <summary>
/// Test per ReportItem - costruttori e proprietà
/// </summary>
public class ReportItemTests
{
    [Test]
    public async Task Constructor_Default_InitializesEmptyProperties()
    {
        // Act
        var item = new ReportItem();

        // Assert
        await Assert.That(item.Label).IsEqualTo(string.Empty);
        await Assert.That(item.Value).IsEqualTo(string.Empty);
        await Assert.That(item.Type).IsEqualTo(ReportItemType.Normal);
        await Assert.That(item.IndentLevel).IsEqualTo(0);
    }

    [Test]
    public async Task Constructor_WithLabelAndValue_SetsBoth()
    {
        // Arrange
        var label = "Format";
        var value = "H.264";

        // Act
        var item = new ReportItem(label, value);

        // Assert
        await Assert.That(item.Label).IsEqualTo(label);
        await Assert.That(item.Value).IsEqualTo(value);
        await Assert.That(item.Type).IsEqualTo(ReportItemType.Normal);
        await Assert.That(item.IndentLevel).IsEqualTo(0);
    }

    [Test]
    public async Task Constructor_WithType_SetsType()
    {
        // Arrange
        var label = "Video";
        var value = "1920x1080";
        var type = ReportItemType.Header;

        // Act
        var item = new ReportItem(label, value, type);

        // Assert
        await Assert.That(item.Label).IsEqualTo(label);
        await Assert.That(item.Value).IsEqualTo(value);
        await Assert.That(item.Type).IsEqualTo(type);
        await Assert.That(item.IndentLevel).IsEqualTo(0);
    }

    [Test]
    public async Task Constructor_WithIndentLevel_SetsIndentLevel()
    {
        // Arrange
        var label = "HDR Format";
        var value = "HDR10";
        var type = ReportItemType.Subheader;
        var indentLevel = 2;

        // Act
        var item = new ReportItem(label, value, type, indentLevel);

        // Assert
        await Assert.That(item.Label).IsEqualTo(label);
        await Assert.That(item.Value).IsEqualTo(value);
        await Assert.That(item.Type).IsEqualTo(type);
        await Assert.That(item.IndentLevel).IsEqualTo(indentLevel);
    }

    [Test]
    public async Task Constructor_AllTypes_WorkCorrectly()
    {
        // Act & Assert
        var normal = new ReportItem("Label", "Value", ReportItemType.Normal);
        await Assert.That(normal.Type).IsEqualTo(ReportItemType.Normal);

        var header = new ReportItem("Label", "Value", ReportItemType.Header);
        await Assert.That(header.Type).IsEqualTo(ReportItemType.Header);

        var subheader = new ReportItem("Label", "Value", ReportItemType.Subheader);
        await Assert.That(subheader.Type).IsEqualTo(ReportItemType.Subheader);

        var separator = new ReportItem("Label", "Value", ReportItemType.Separator);
        await Assert.That(separator.Type).IsEqualTo(ReportItemType.Separator);
    }

    [Test]
    public async Task Constructor_EmptyStrings_AcceptsEmptyStrings()
    {
        // Act
        var item = new ReportItem("", "");

        // Assert
        await Assert.That(item.Label).IsEqualTo(string.Empty);
        await Assert.That(item.Value).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task Constructor_VariousIndentLevels_SetsCorrectly()
    {
        // Act & Assert
        var level0 = new ReportItem("L", "V", ReportItemType.Normal, 0);
        await Assert.That(level0.IndentLevel).IsEqualTo(0);

        var level1 = new ReportItem("L", "V", ReportItemType.Normal, 1);
        await Assert.That(level1.IndentLevel).IsEqualTo(1);

        var level5 = new ReportItem("L", "V", ReportItemType.Normal, 5);
        await Assert.That(level5.IndentLevel).IsEqualTo(5);
    }
}
