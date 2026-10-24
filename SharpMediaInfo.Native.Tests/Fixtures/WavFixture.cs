using System.Text;

namespace SharpMediaInfo.Native.Tests.Fixtures;

/// <summary>
/// Generates a minimal, valid PCM WAV file in memory so the native tests have a
/// deterministic media sample without shipping a binary fixture in the repo.
/// MediaInfo recognises this as General/Format = "Wave" with one Audio (PCM) stream.
/// </summary>
internal static class WavFixture
{
    public const int SampleRate = 8000;
    public const short Channels = 1;
    public const short BitsPerSample = 16;
    public const int SampleCount = 8000; // 1 second of audio

    public static byte[] CreatePcmWav()
    {
        int blockAlign = Channels * BitsPerSample / 8;
        int byteRate = SampleRate * blockAlign;
        int dataSize = SampleCount * blockAlign;

        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms, Encoding.ASCII, leaveOpen: true);

        w.Write(Encoding.ASCII.GetBytes("RIFF"));
        w.Write(36 + dataSize);
        w.Write(Encoding.ASCII.GetBytes("WAVE"));

        w.Write(Encoding.ASCII.GetBytes("fmt "));
        w.Write(16);                 // PCM fmt chunk size
        w.Write((short)1);           // AudioFormat = PCM
        w.Write(Channels);
        w.Write(SampleRate);
        w.Write(byteRate);
        w.Write((short)blockAlign);
        w.Write(BitsPerSample);

        w.Write(Encoding.ASCII.GetBytes("data"));
        w.Write(dataSize);
        for (int i = 0; i < SampleCount; i++)
        {
            // A simple low-amplitude ramp; content is irrelevant to MediaInfo.
            w.Write((short)(i % 256));
        }

        w.Flush();
        return ms.ToArray();
    }

    /// <summary>
    /// Writes a WAV file to a unique temporary path and returns it.
    /// The caller is responsible for deleting the file.
    /// </summary>
    public static string CreateTempWavFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"smi_test_{Guid.NewGuid():N}.wav");
        File.WriteAllBytes(path, CreatePcmWav());
        return path;
    }
}
