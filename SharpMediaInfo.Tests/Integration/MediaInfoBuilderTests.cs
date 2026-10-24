using SharpMediaInfo.Tests.Fixtures;

namespace SharpMediaInfo.Tests.Integration;

/// <summary>
/// End-to-end tests covering the full stack: <see cref="MediaInfoBuilder"/> →
/// native MediaInfo → source-generated typed tracks.
/// </summary>
public class MediaInfoBuilderTests
{
    [Test]
    public async Task FromFile_Wav_ParsesGeneralAndAudioTracks()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            var media = MediaInfoBuilder.FromFile(path).Build();

            await Assert.That(media.InfoAvailable()).IsTrue();
            await Assert.That(media.GeneralCount).IsEqualTo(1);
            await Assert.That(media.AudioCount).IsEqualTo(1);
            await Assert.That(media.General[0].Format).IsEqualTo("Wave");
            await Assert.That(media.Audio[0].Format).IsEqualTo("PCM");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task FromFile_Wav_MapsChannelsToTypedValue()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            var media = MediaInfoBuilder.FromFile(path).Build();

            await Assert.That(media.Audio[0].Channels.Count).IsEqualTo((long)WavFixture.Channels);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task FromStream_Wav_Parses()
    {
        using var stream = new MemoryStream(WavFixture.CreatePcmWav());

        var media = MediaInfoBuilder.FromStream(stream).Build();

        await Assert.That(media.AudioCount).IsEqualTo(1);
    }

    [Test]
    public async Task WithReports_PopulatesStandardReport()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            var media = MediaInfoBuilder.FromFile(path).WithReports().Build();

            await Assert.That(media.InfoStandard).IsNotNull();
            await Assert.That(media.InfoStandard!).Contains("Audio");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task InfoCustom_ListsEveryStreamSection()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            var media = MediaInfoBuilder.FromFile(path).Build();

            await Assert.That(media.InfoCustom).Contains("General");
            await Assert.That(media.InfoCustom).Contains("Audio");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task FromCopy_RoundTripsCountsAndTracks()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            var original = MediaInfoBuilder.FromFile(path).Build();

            var copy = MediaInfoBuilder.FromCopy(original).Build();

            await Assert.That(copy.AudioCount).IsEqualTo(original.AudioCount);
            await Assert.That(copy.GeneralCount).IsEqualTo(original.GeneralCount);
            await Assert.That(copy.Audio[0].Format).IsEqualTo(original.Audio[0].Format);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task FromFile_MissingFile_Throws()
    {
        await Assert.That(() => MediaInfoBuilder.FromFile(@"X:\does\not\exist.wav"))
            .Throws<FileNotFoundException>();
    }

    [Test]
    public async Task FromStream_EmptyStream_Throws()
    {
        using var empty = new MemoryStream();

        await Assert.That(() => MediaInfoBuilder.FromStream(empty))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task Build_Twice_Throws()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            var builder = MediaInfoBuilder.FromFile(path);
            builder.Build();

            await Assert.That(() => builder.Build()).Throws<System.Exception>();
        }
        finally
        {
            File.Delete(path);
        }
    }
}
