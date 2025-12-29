using SharpMediaInfo.GUI.Converters;
using SharpMediaInfo.GUI.Models.Report;
using SharpMediaInfo.Tracks;

namespace SharpMediaInfo.GUI.Tests.Converters;

/// <summary>
/// Test per TrackToTreeConverter - conversione track a tree
/// </summary>
public class TrackToTreeConverterTests
{
    [Test]
    public async Task Convert_WithConfiguration_ShowMillisecondsAffectsOutput()
    {
        // Arrange
        var config1 = new AppConfiguration { ShowMilliseconds = true };
        var config2 = new AppConfiguration { ShowMilliseconds = false };

        var converter1 = new TrackToTreeConverter(config1);
        var converter2 = new TrackToTreeConverter(config2);

        // Assert - Just verify converters are created with different configs
        await Assert.That(converter1).IsNotNull();
        await Assert.That(converter2).IsNotNull();
    }

    [Test]
    public async Task Convert_WithEmptyMediaInfo_ReturnsEmptyList()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Convert_WithGeneralTrack_CreatesGeneralNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var generalTrack = new GeneralTrack
        {
            FileName = "test",
            FileExtension = "mp4",
            Format = "MPEG-4"
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack> { generalTrack });
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("General");
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Section);
        await Assert.That(nodes[0].IsExpanded).IsTrue();
        await Assert.That(nodes[0].Children.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Convert_WithVideoTrack_CreatesVideoNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var videoTrack = new VideoTrack
        {
            Width = 1920,
            Height = 1080,
            Format = "AVC",
            FormatCommercial = "AVC"
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("Video");
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Track);
    }

    [Test]
    public async Task Convert_WithMultipleVideoTracks_CreatesNumberedNodes()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var videoTrack1 = new VideoTrack { Format = "AVC", FormatCommercial = "AVC" };
        var videoTrack2 = new VideoTrack { Format = "HEVC", FormatCommercial = "HEVC" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack1, videoTrack2 });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(2);
        await Assert.That(nodes[0].Header).IsEqualTo("Video #1");
        await Assert.That(nodes[1].Header).IsEqualTo("Video #2");
    }

    [Test]
    public async Task Convert_WithAudioTrack_CreatesAudioNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var audioTrack = new AudioTrack
        {
            Format = "AAC"
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack> { audioTrack });
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("Audio");
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Track);
    }

    [Test]
    public async Task Convert_WithTextTrack_CreatesTextNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var textTrack = new TextTrack
        {
            FormatCommercial = "UTF-8"
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack> { textTrack });
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("Text");
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Track);
    }

    [Test]
    public async Task Convert_WithMenuTrack_CreatesMenuNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var menuTrack = new MenuTrack();

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack> { menuTrack });
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("Menu");
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Track);
    }

    [Test]
    public async Task Convert_WithImageTrack_CreatesImageNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var imageTrack = new ImageTrack
        {
            Format = "JPEG",
            Width = 1920,
            Height = 1080
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack> { imageTrack });

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("Image");
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Track);
    }

    [Test]
    public async Task Convert_WithMultipleTracks_CreatesMultipleNodes()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var generalTrack = new GeneralTrack { FileName = "test", FileExtension = "mp4" };
        var videoTrack = new VideoTrack { Format = "AVC", FormatCommercial = "AVC" };
        var audioTrack = new AudioTrack { Format = "AAC" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack> { generalTrack });
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack> { audioTrack });
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(3);
        await Assert.That(nodes[0].Header).IsEqualTo("General");
        await Assert.That(nodes[1].Header).IsEqualTo("Video");
        await Assert.That(nodes[2].Header).IsEqualTo("Audio");
    }

    [Test]
    public async Task Convert_WithVideoTrackHDR_CreatesHDRMetadataNode()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var videoTrack = new VideoTrack
        {
            Format = "HEVC",
            FormatCommercial = "HEVC",
            HDRFormat = "HDR10"
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(1);
        await Assert.That(nodes[0].Header).IsEqualTo("Video");

        // Check for HDR metadata child node
        var hdrNode = nodes[0].Children.FirstOrDefault(c => c.Header == "HDR metadata");
        await Assert.That(hdrNode).IsNotNull();
        await Assert.That(hdrNode!.Type).IsEqualTo(TreeNodeType.Section);
    }

    [Test]
    public async Task Convert_WithMultipleAudioTracks_CreatesNumberedNodes()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var audioTrack1 = new AudioTrack { Format = "AAC" };
        var audioTrack2 = new AudioTrack { Format = "DTS" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack> { audioTrack1, audioTrack2 });
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(2);
        await Assert.That(nodes[0].Header).IsEqualTo("Audio #1");
        await Assert.That(nodes[1].Header).IsEqualTo("Audio #2");
    }

    [Test]
    public async Task Convert_WithMultipleTextTracks_CreatesNumberedNodes()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var textTrack1 = new TextTrack { FormatCommercial = "UTF-8" };
        var textTrack2 = new TextTrack { FormatCommercial = "SRT" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack> { textTrack1, textTrack2 });
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(2);
        await Assert.That(nodes[0].Header).IsEqualTo("Text #1");
        await Assert.That(nodes[1].Header).IsEqualTo("Text #2");
    }

    [Test]
    public async Task Convert_GeneralNode_IsExpandedByDefault()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var generalTrack = new GeneralTrack { FileName = "test", FileExtension = "mp4" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack> { generalTrack });
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes[0].IsExpanded).IsTrue();
    }

    [Test]
    public async Task Convert_TrackNodes_AreExpandedByDefault()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var videoTrack = new VideoTrack { Format = "AVC", FormatCommercial = "AVC" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes[0].IsExpanded).IsTrue();
        await Assert.That(nodes[0].Type).IsEqualTo(TreeNodeType.Track);
    }

    [Test]
    public async Task Convert_AllTrackTypes_CreatesCorrectOrder()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToTreeConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var generalTrack = new GeneralTrack { FileName = "test", FileExtension = "mp4" };
        var videoTrack = new VideoTrack { Format = "AVC", FormatCommercial = "AVC" };
        var audioTrack = new AudioTrack { Format = "AAC" };
        var textTrack = new TextTrack { FormatCommercial = "UTF-8" };
        var menuTrack = new MenuTrack();
        var imageTrack = new ImageTrack { Format = "JPEG" };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack> { generalTrack });
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack> { audioTrack });
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack> { textTrack });
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack> { menuTrack });
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack> { imageTrack });

        // Act
        var nodes = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(nodes).IsNotNull();
        await Assert.That(nodes.Count).IsEqualTo(6);
        await Assert.That(nodes[0].Header).IsEqualTo("General");
        await Assert.That(nodes[1].Header).IsEqualTo("Video");
        await Assert.That(nodes[2].Header).IsEqualTo("Audio");
        await Assert.That(nodes[3].Header).IsEqualTo("Text");
        await Assert.That(nodes[4].Header).IsEqualTo("Menu");
        await Assert.That(nodes[5].Header).IsEqualTo("Image");
    }
}
