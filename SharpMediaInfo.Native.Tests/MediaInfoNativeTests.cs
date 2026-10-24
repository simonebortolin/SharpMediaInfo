using SharpMediaInfo.Native.Exception;
using SharpMediaInfo.Native.Tests.Fixtures;

namespace SharpMediaInfo.Native.Tests;

/// <summary>
/// End-to-end tests against the native library using an in-memory generated WAV file.
/// </summary>
public class MediaInfoNativeTests
{
    [Test]
    public async Task OpenFile_Wav_ReportsGeneralAndAudioStreams()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            using var native = IMediaInfoNative.Create(parseCoverData: false);
            native.OpenFile(path);

            await Assert.That(native.GetStreamCount(StreamKind.General)).IsGreaterThanOrEqualTo(1);
            await Assert.That(native.GetStreamCount(StreamKind.Audio)).IsGreaterThanOrEqualTo(1);

            var format = native.GetParameter(StreamKind.General, 0, "Format");
            await Assert.That(format).IsEqualTo("Wave");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task OpenFile_Wav_ExposesAudioParameters()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            using var native = IMediaInfoNative.Create(parseCoverData: false);
            native.OpenFile(path);

            var channels = native.GetParameter(StreamKind.Audio, 0, "Channel(s)");
            var sampleRate = native.GetParameter(StreamKind.Audio, 0, "SamplingRate");

            await Assert.That(channels).IsEqualTo(WavFixture.Channels.ToString());
            await Assert.That(sampleRate).IsEqualTo(WavFixture.SampleRate.ToString());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task OpenStream_Wav_ReportsAudioStream()
    {
        using var stream = new MemoryStream(WavFixture.CreatePcmWav());
        using var native = IMediaInfoNative.Create(parseCoverData: false);

        native.OpenStream(stream);

        await Assert.That(native.GetStreamCount(StreamKind.Audio)).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task StandardReport_IsProduced()
    {
        var path = WavFixture.CreateTempWavFile();
        try
        {
            using var native = IMediaInfoNative.Create(parseCoverData: false);
            native.OpenFile(path);

            var report = native.GetStandardReport();

            await Assert.That(report).IsNotNull();
            await Assert.That(report!).Contains("Audio");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task OpenStream_EmptyStream_Throws()
    {
        using var empty = new MemoryStream();
        using var native = IMediaInfoNative.Create(parseCoverData: false);

        await Assert.That(() => native.OpenStream(empty)).Throws<ArgumentException>();
    }

    [Test]
    public async Task GetStreamCount_BeforeOpen_Throws()
    {
        using var native = IMediaInfoNative.Create(parseCoverData: false);

        await Assert.That(() => native.GetStreamCount(StreamKind.General))
            .Throws<MediaInfoNativeNotOpenException>();
    }

    [Test]
    public async Task Dispose_IsIdempotent()
    {
        var native = IMediaInfoNative.Create(parseCoverData: false);
        native.OpenStream(new MemoryStream(WavFixture.CreatePcmWav()));

        native.Dispose();
        native.Dispose(); // must not throw
    }
}
