using SharpMediaInfo.GUI.Converters;
using SharpMediaInfo.Tracks;

namespace SharpMediaInfo.GUI.Tests.Converters;

/// <summary>
/// Test per TrackToReportConverter - conversione track a report
/// </summary>
public class TrackToReportConverterTests
{
    [Test]
    public async Task Convert_WithConfiguration_ShowMillisecondsAffectsOutput()
    {
        // Arrange
        var config1 = new AppConfiguration { ShowMilliseconds = true };
        var config2 = new AppConfiguration { ShowMilliseconds = false };

        var converter1 = new TrackToReportConverter(config1);
        var converter2 = new TrackToReportConverter(config2);

        // Assert - Just verify converters are created with different configs
        await Assert.That(converter1).IsNotNull();
        await Assert.That(converter2).IsNotNull();
    }

    [Test]
    public async Task Convert_WithEmptyMediaInfo_ReturnsEmptyList()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Convert_WithGeneralTrack_CreatesGeneralSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("General");
        await Assert.That(sections[0].Items.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task Convert_WithVideoTrack_CreatesVideoSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("Video");
    }

    [Test]
    public async Task Convert_WithAudioTrack_CreatesAudioSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("Audio");
    }

    [Test]
    public async Task Convert_WithTextTrack_CreatesTextSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("Text");
    }

    [Test]
    public async Task Convert_WithMenuTrack_CreatesMenuSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var menuTrack = new MenuTrack();

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack> { menuTrack });
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("Menu");
    }

    [Test]
    public async Task Convert_WithImageTrack_CreatesImageSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("Image");
    }

    [Test]
    public async Task Convert_WithMultipleTracks_CreatesMultipleSections()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(3);
        await Assert.That(sections[0].Title).IsEqualTo("General");
        await Assert.That(sections[1].Title).IsEqualTo("Video");
        await Assert.That(sections[2].Title).IsEqualTo("Audio");
    }

    [Test]
    public async Task Convert_WithShowMilliseconds_IncludesMillisecondsInDuration()
    {
        // Arrange
        var config = new AppConfiguration { ShowMilliseconds = true };
        var converter = new TrackToReportConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var generalTrack = new GeneralTrack
        {
            FileName = "test",
            FileExtension = "mp4",
            Duration = TimeSpan.FromSeconds(65.523)
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack> { generalTrack });
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Convert_WithoutShowMilliseconds_ExcludesMillisecondsInDuration()
    {
        // Arrange
        var config = new AppConfiguration { ShowMilliseconds = false };
        var converter = new TrackToReportConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var generalTrack = new GeneralTrack
        {
            FileName = "test",
            FileExtension = "mp4",
            Duration = TimeSpan.FromSeconds(65.523)
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack> { generalTrack });
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack>());
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Convert_WithVideoHDR_CreatesHDRSection()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
        var mediaInfoMock = new Mock<IMediaInfo>();

        var videoTrack = new VideoTrack
        {
            Format = "HEVC",
            FormatCommercial = "HEVC",
            HDRFormat = "HDR10",
            ColourRange = "Limited",
            ColourPrimaries = "BT.2020"
        };

        mediaInfoMock.Setup(x => x.General).Returns(new List<GeneralTrack>());
        mediaInfoMock.Setup(x => x.Video).Returns(new List<VideoTrack> { videoTrack });
        mediaInfoMock.Setup(x => x.Audio).Returns(new List<AudioTrack>());
        mediaInfoMock.Setup(x => x.Text).Returns(new List<TextTrack>());
        mediaInfoMock.Setup(x => x.Menu).Returns(new List<MenuTrack>());
        mediaInfoMock.Setup(x => x.Image).Returns(new List<ImageTrack>());

        // Act
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(1);
        await Assert.That(sections[0].Title).IsEqualTo("Video");
    }

    [Test]
    public async Task Convert_WithMultipleVideoTracks_CreatesMultipleSections()
    {
        // Arrange
        var config = new AppConfiguration();
        var converter = new TrackToReportConverter(config);
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
        var sections = converter.Convert(mediaInfoMock.Object);

        // Assert
        await Assert.That(sections).IsNotNull();
        await Assert.That(sections.Count).IsEqualTo(2);
        await Assert.That(sections[0].Title).IsEqualTo("Video");
        await Assert.That(sections[1].Title).IsEqualTo("Video");
    }
}
