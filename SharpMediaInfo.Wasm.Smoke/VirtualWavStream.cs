using System.Text;

namespace SharpMediaInfo.Wasm.Smoke;

/// <summary>
/// A seekable, read-only stream that presents a PCM WAV of arbitrary size without
/// allocating it: the 44-byte header is real, the sample data is generated on the fly
/// as zeros. An optional title is stored in a LIST/INFO chunk placed *after* the data, so
/// MediaInfo has to jump over the data to find it. It records how many bytes were read and
/// how many seeks happened, so tests can prove the analysis reads only what MediaInfo asks
/// for instead of the whole file.
/// </summary>
internal sealed class VirtualWavStream : Stream
{
    private readonly byte[] _header;
    private readonly byte[] _trailer;
    private readonly long _trailerStart;
    private long _position;

    public VirtualWavStream(long dataSize, int sampleRate = 48000, short channels = 2, short bitsPerSample = 16, string? title = null)
    {
        if (dataSize <= 0 || dataSize % 2 != 0 || dataSize > uint.MaxValue - 1024)
            throw new ArgumentOutOfRangeException(nameof(dataSize));

        _trailer = title is null ? [] : CreateInfoChunk(title);

        int blockAlign = channels * bitsPerSample / 8;
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.ASCII, leaveOpen: true))
        {
            w.Write(Encoding.ASCII.GetBytes("RIFF"));
            w.Write((uint)(36 + dataSize + _trailer.Length));
            w.Write(Encoding.ASCII.GetBytes("WAVE"));
            w.Write(Encoding.ASCII.GetBytes("fmt "));
            w.Write(16);
            w.Write((short)1); // PCM
            w.Write(channels);
            w.Write(sampleRate);
            w.Write(sampleRate * blockAlign);
            w.Write((short)blockAlign);
            w.Write(bitsPerSample);
            w.Write(Encoding.ASCII.GetBytes("data"));
            w.Write((uint)dataSize);
        }
        _header = ms.ToArray();
        _trailerStart = _header.Length + dataSize;
        Length = _trailerStart + _trailer.Length;
    }

    // LIST/INFO chunk with a single INAM (title) entry.
    private static byte[] CreateInfoChunk(string title)
    {
        var text = Encoding.ASCII.GetBytes(title + "\0");
        int padded = text.Length + (text.Length % 2);
        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.ASCII, leaveOpen: true))
        {
            w.Write(Encoding.ASCII.GetBytes("LIST"));
            w.Write(4 + 8 + padded);
            w.Write(Encoding.ASCII.GetBytes("INFO"));
            w.Write(Encoding.ASCII.GetBytes("INAM"));
            w.Write(text.Length);
            w.Write(text);
            if (padded != text.Length)
                w.Write((byte)0);
        }
        return ms.ToArray();
    }

    /// <summary>A complete one-second 8 kHz mono WAV, materialized in memory.</summary>
    public static byte[] CreateSmall()
    {
        using var source = new VirtualWavStream(16000, sampleRate: 8000, channels: 1);
        var bytes = new byte[source.Length];
        source.ReadExactly(bytes);
        return bytes;
    }

    /// <summary>Total bytes handed out by Read/ReadAsync.</summary>
    public long BytesRead { get; private set; }

    /// <summary>Number of Seek calls / Position assignments that moved the stream.</summary>
    public int SeekCount { get; private set; }

    /// <summary>Number of ReadAsync calls (to tell the async path was used).</summary>
    public int AsyncReadCount { get; private set; }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length { get; }

    public override long Position
    {
        get => _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

    public override int Read(Span<byte> destination)
    {
        int count = (int)Math.Min(destination.Length, Length - _position);
        if (count <= 0)
            return 0;

        destination[..count].Clear();
        CopyOverlap(_header, 0, destination[..count]);
        CopyOverlap(_trailer, _trailerStart, destination[..count]);

        _position += count;
        BytesRead += count;
        return count;
    }

    // Copies the part of `source` (located at `sourceStart` in the stream) that overlaps the
    // window [_position, _position + destination.Length).
    private void CopyOverlap(byte[] source, long sourceStart, Span<byte> destination)
    {
        long from = Math.Max(_position, sourceStart);
        long to = Math.Min(_position + destination.Length, sourceStart + source.Length);
        if (from < to)
            source.AsSpan((int)(from - sourceStart), (int)(to - from)).CopyTo(destination[(int)(from - _position)..]);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        AsyncReadCount++;
        await Task.Yield(); // behave like real asynchronous I/O
        return Read(buffer.Span);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        long target = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => Length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin)),
        };
        if (target < 0)
            throw new IOException("Seek before the beginning of the stream.");
        if (target != _position)
            SeekCount++;
        _position = target;
        return _position;
    }

    public override void Flush() { }
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
