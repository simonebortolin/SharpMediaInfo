using SharpMediaInfo.GUI.Models.Report;

namespace SharpMediaInfo.GUI.Tests.Models.Report;

/// <summary>
/// Test per TreeNode - costruttori e proprietà
/// </summary>
public class TreeNodeTests
{
    [Test]
    public async Task Constructor_Default_InitializesEmptyProperties()
    {
        // Act
        var node = new TreeNode();

        // Assert
        await Assert.That(node.Header).IsEqualTo(string.Empty);
        await Assert.That(node.Value).IsNull();
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children.Count).IsEqualTo(0);
        await Assert.That(node.IsExpanded).IsFalse();
        await Assert.That(node.Type).IsEqualTo(TreeNodeType.Normal);
    }

    [Test]
    public async Task Constructor_WithHeader_SetsHeader()
    {
        // Arrange
        var header = "General";

        // Act
        var node = new TreeNode(header);

        // Assert
        await Assert.That(node.Header).IsEqualTo(header);
        await Assert.That(node.Value).IsNull();
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children.Count).IsEqualTo(0);
        await Assert.That(node.IsExpanded).IsFalse();
    }

    [Test]
    public async Task Constructor_WithHeaderAndValue_SetsBoth()
    {
        // Arrange
        var header = "Format";
        var value = "MP4";

        // Act
        var node = new TreeNode(header, value);

        // Assert
        await Assert.That(node.Header).IsEqualTo(header);
        await Assert.That(node.Value).IsEqualTo(value);
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Constructor_WithChildren_SetsChildren()
    {
        // Arrange
        var header = "Video";
        var children = new List<TreeNode>
        {
            new TreeNode("Codec", "H.264"),
            new TreeNode("Resolution", "1920x1080"),
            new TreeNode("Framerate", "30 fps")
        };

        // Act
        var node = new TreeNode(header, children);

        // Assert
        await Assert.That(node.Header).IsEqualTo(header);
        await Assert.That(node.Value).IsNull();
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children.Count).IsEqualTo(3);
        await Assert.That(node.Children[0].Header).IsEqualTo("Codec");
        await Assert.That(node.Children[0].Value).IsEqualTo("H.264");
        await Assert.That(node.Children[1].Header).IsEqualTo("Resolution");
        await Assert.That(node.Children[1].Value).IsEqualTo("1920x1080");
        await Assert.That(node.Children[2].Header).IsEqualTo("Framerate");
        await Assert.That(node.Children[2].Value).IsEqualTo("30 fps");
    }

    [Test]
    public async Task IsExpanded_SetTrue_UpdatesProperty()
    {
        // Arrange
        var node = new TreeNode("Audio");

        // Act
        node.IsExpanded = true;

        // Assert
        await Assert.That(node.IsExpanded).IsTrue();
    }

    [Test]
    public async Task IsExpanded_SetFalse_UpdatesProperty()
    {
        // Arrange
        var node = new TreeNode("Text") { IsExpanded = true };

        // Act
        node.IsExpanded = false;

        // Assert
        await Assert.That(node.IsExpanded).IsFalse();
    }

    [Test]
    public async Task Type_AllTypes_WorkCorrectly()
    {
        // Act & Assert
        var normal = new TreeNode { Type = TreeNodeType.Normal };
        await Assert.That(normal.Type).IsEqualTo(TreeNodeType.Normal);

        var section = new TreeNode { Type = TreeNodeType.Section };
        await Assert.That(section.Type).IsEqualTo(TreeNodeType.Section);

        var track = new TreeNode { Type = TreeNodeType.Track };
        await Assert.That(track.Type).IsEqualTo(TreeNodeType.Track);

        var property = new TreeNode { Type = TreeNodeType.Property };
        await Assert.That(property.Type).IsEqualTo(TreeNodeType.Property);
    }

    [Test]
    public async Task Constructor_NestedChildren_CreatesHierarchy()
    {
        // Arrange
        var grandChildren = new List<TreeNode>
        {
            new TreeNode("Bit Depth", "10-bit"),
            new TreeNode("Color Space", "BT.2020")
        };

        var children = new List<TreeNode>
        {
            new TreeNode("Basic Info", new List<TreeNode> { new TreeNode("Codec", "HEVC") }),
            new TreeNode("HDR Metadata", grandChildren)
        };

        // Act
        var root = new TreeNode("Video Track", children);

        // Assert
        await Assert.That(root.Header).IsEqualTo("Video Track");
        await Assert.That(root.Children.Count).IsEqualTo(2);
        await Assert.That(root.Children[0].Header).IsEqualTo("Basic Info");
        await Assert.That(root.Children[0].Children.Count).IsEqualTo(1);
        await Assert.That(root.Children[1].Header).IsEqualTo("HDR Metadata");
        await Assert.That(root.Children[1].Children.Count).IsEqualTo(2);
        await Assert.That(root.Children[1].Children[0].Header).IsEqualTo("Bit Depth");
        await Assert.That(root.Children[1].Children[0].Value).IsEqualTo("10-bit");
    }

    [Test]
    public async Task Constructor_EmptyChildren_CreatesEmptyList()
    {
        // Act
        var node = new TreeNode("Menu", new List<TreeNode>());

        // Assert
        await Assert.That(node.Header).IsEqualTo("Menu");
        await Assert.That(node.Children).IsNotNull();
        await Assert.That(node.Children.Count).IsEqualTo(0);
    }
}
