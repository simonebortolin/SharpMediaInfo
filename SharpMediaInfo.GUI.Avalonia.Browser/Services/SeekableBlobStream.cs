using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpMediaInfo.GUI.Avalonia.Browser.Services;

/// <summary>
/// Avalonia's browser file stream reads with <c>Blob.slice</c>, so it supports random access through
/// <see cref="Stream.Seek"/> and knows its <see cref="Stream.Length"/>, but reports
/// <see cref="Stream.CanSeek"/> = false and rejects the <see cref="Stream.Position"/> setter.
/// MediaInfo needs both, so this wrapper exposes them by translating every move to an absolute seek.
/// </summary>
internal sealed class SeekableBlobStream : Stream
{
    private readonly Stream _inner;
    private readonly long _length;
    private long _position;

    public SeekableBlobStream(Stream inner)
    {
        _inner = inner;
        _length = inner.Length;
    }

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _length;

    public override long Position
    {
        get => _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var target = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin)),
        };
        ArgumentOutOfRangeException.ThrowIfNegative(target, nameof(offset));
        _inner.Seek(target, SeekOrigin.Begin);
        _position = target;
        return target;
    }

    public override int Read(byte[] buffer, int offset, int count)
        => Advance(_inner.Read(buffer, offset, count));

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => Advance(await _inner.ReadAsync(buffer, cancellationToken).ConfigureAwait(false));

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

    private int Advance(int read)
    {
        _position += read;
        return read;
    }

    public override void Flush() { }
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing) _inner.Dispose();
        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync() => _inner.DisposeAsync();
}
