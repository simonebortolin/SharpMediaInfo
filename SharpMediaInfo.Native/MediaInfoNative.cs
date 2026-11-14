using SharpMediaInfo.Exception;
using SharpMediaInfo.Native.Enum;
using SharpMediaInfo.Native.Exception;
using SharpMediaInfo.Native.Interop;
using System.Runtime.InteropServices;

namespace SharpMediaInfo.Native
{
    /// <summary>
    /// Initializes a new instance of the MediaInfoNative class using the specified handle and an option to parse
    /// cover data.
    /// </summary>
    /// <param name="parseCoverData">true to enable parsing of embedded cover image data; otherwise, false. The default is false.</param>
    internal sealed class MediaInfoNative(bool parseCoverData = false) : IMediaInfoNative
    {
        private MediaInfoHandle? handle;
        private bool disposed = false;

        /// <summary>
        /// Static method to get library information (version, capabilities, etc.)
        /// </summary>
        internal static string GetLibraryInfoStatic(string option, string value)
        {
            return MediaInfoNativeMethods.SetOption(IntPtr.Zero, option, value);
        }

        /// <summary>
        /// Gets the value of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns>
        /// A string containing the parameter's value or an empty string in case of problems.
        /// </returns>
        public string GetParameter(StreamKind kind, int trackIndex, string parameterName)
        {
            return MediaInfoNativeMethods.GetParameter(
                handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(),
                kind,
                (uint)trackIndex,
                parameterName,
                InfoKind.Text,
                InfoKind.Name);
        }

        /// <summary>
        /// Gets the value of a property of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterIndex">The parameter index</param>
        /// <param name="infoKind">the information kind</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        public string GetSpecificParameter(
            StreamKind kind,
            int trackIndex,
            int parameterIndex,
            InfoKind infoKind)
        {
            return MediaInfoNativeMethods.GetParameter(
                handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(),
                kind,
                (uint)trackIndex,
                (uint)parameterIndex,
                infoKind);
        }

        /// <summary>
        /// Configures whether cover image data should be parsed as base64-encoded data.
        /// </summary>
        /// <remarks>Call this method before retrieving cover image data to specify the desired parsing
        /// format. This setting affects how cover data is returned by subsequent operations.</remarks>
        /// <param name="flag">true to enable parsing of cover data as base64; false to disable parsing.</param>
        public void SetCoverDataParsing(bool flag)
        {
            // Always set the option when parseCoverData was enabled, regardless of flag value
            // This allows toggling between base64 mode (for cover data) and empty mode (for reports)
            var value = parseCoverData && flag ? "base64" : "";
            MediaInfoNativeMethods.SetOption(
                handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(),
                "Cover_Data",
                value);
        }

        /// <summary>
        /// Lists media info capacities.
        /// </summary>
        /// <returns>The string that represents the formatted list of Mediainfo library capacities.</returns>
        public static string? Capacities()
        {
            return MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Capacities", "");
        }

        /// <summary>
        /// Lists all info parameters of the MediaInfo library.
        /// </summary>
        /// <returns>The string that represents the formatted list of all available parameters.</returns>
        public static string? ParametersList(bool csvFormat = false)
        {
            return MediaInfoNativeMethods.SetOption(IntPtr.Zero, csvFormat ? "Info_Parameters_CSV" : "Info_Parameters", "");
        }

        /// <summary>
        /// Lists all supported codecs.
        /// </summary>
        /// <returns>The string that represents the formatted list of all supported codecs</returns>
        public static string? KnownCodecs()
        {
            return MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Codecs", "");
        }

        /// <summary>
        /// MediaInfo library version.
        /// </summary>
        /// <returns>The library version</returns>
        public static string? LibraryVersion()
        {
            return MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Version", "");
        }

        /// <summary>
        /// MediaInfo library url.
        /// </summary>
        /// <returns>The library url</returns>
        public static string? LibraryUrl()
        {
            return MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Url", "");
        }

        /// <summary>
        /// Gets stream count for a defined kind.
        /// </summary>
        /// <param name="kind">The defined kind</param>
        /// <returns>The stream count.</returns>
        public int GetStreamCount(StreamKind kind)
        {
            return MediaInfoNativeMethods.GetStreamCount(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(), kind, -1);
        }

        /// <summary>
        /// Gets the complete report.
        /// </summary>
        public string? GetCompleteReport()
        {
            MediaInfoNativeMethods.SetOption(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(), "Complete", "1");
            SetCoverDataParsing(false);
            var report = MediaInfoNativeMethods.Inform(handle.DangerousGetHandle());
            SetCoverDataParsing(parseCoverData);
            return report;
        }

        /// <summary>
        /// Gets the standard report.
        /// </summary>
        public string? GetStandardReport()
        {
            MediaInfoNativeMethods.SetOption(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(), "Complete", "");
            SetCoverDataParsing(false);
            var report = MediaInfoNativeMethods.Inform(handle.DangerousGetHandle());
            SetCoverDataParsing(parseCoverData);
            return report;
        }

        public void Close()
        {
            MediaInfoNativeMethods.CloseFile(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException());
        }

        public void OpenFile(string path)
        {
            handle = new MediaInfoHandle();

            SetCoverDataParsing(parseCoverData);

            //opens the handle
            if (!MediaInfoNativeMethods.OpenFile(handle.DangerousGetHandle(), path))
            {
                throw new MediaInfoNativeOpenException("Unable to open the file.");
            }
        }

        /// <summary>
        /// Size of each chunk fed to the library. MediaInfo only needs a few of them per file
        /// (headers, an index, maybe the tail), jumping around with seek requests.
        /// </summary>
        internal const int StreamChunkSize = 256 * 1024;

        public void OpenStream(Stream stream)
        {
            var reader = BeginStream(stream);
            while (true)
            {
                int read = stream.Read(reader.Buffer, 0, reader.Buffer.Length);
                if (!reader.Feed(read))
                    break;
            }
            reader.Finish();
        }

        public async Task OpenStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var reader = BeginStream(stream);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int read = await stream.ReadAsync(reader.Buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                if (!reader.Feed(read))
                    break;
            }
            reader.Finish();
        }

        private BufferFeeder BeginStream(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead || !stream.CanSeek || stream.Length <= 0)
            {
                throw new ArgumentException("The stream cannot be read or it's empty");
            }

            handle = new MediaInfoHandle();
            SetCoverDataParsing(parseCoverData);

            var reader = new BufferFeeder(handle.DangerousGetHandle(), stream);
            reader.Start();
            return reader;
        }

        /// <summary>
        /// Drives MediaInfo's buffer API over a seekable stream without loading it in memory:
        /// each chunk is passed to <c>Open_Buffer_Continue</c>, and whenever the library asks
        /// for another position (<c>Open_Buffer_Continue_GoTo_Get</c>) the stream is moved there
        /// and the parser re-initialised at that offset, exactly like MediaInfo's own file reader.
        /// Only the reads differ between the sync and async paths; all native calls live here,
        /// outside any <c>await</c>, so no buffer stays pinned across a suspension.
        /// </summary>
        private sealed class BufferFeeder(IntPtr handle, Stream stream)
        {
            // Bit 3 of the Open_Buffer_Continue status: the parser has everything it needs.
            private const long StatusFinalized = 0x08;

            public byte[] Buffer { get; } = new byte[(int)Math.Min(StreamChunkSize, stream.Length)];

            public void Start()
            {
                stream.Seek(0, SeekOrigin.Begin);
                if (!MediaInfoNativeMethods.InitializeBuffer(handle, stream.Length, 0))
                {
                    throw new MediaInfoNativeOpenException("Unable to open the stream.");
                }
            }

            /// <summary>Feeds <paramref name="count"/> bytes; returns false when parsing is done.</summary>
            public unsafe bool Feed(int count)
            {
                if (count <= 0)
                    return false; // end of stream

                nint status;
                fixed (byte* data = Buffer)
                {
                    status = MediaInfoNativeMethods.ReadBuffer(handle, (IntPtr)data, (nuint)count);
                }
                if ((status & StatusFinalized) != 0)
                    return false;

                long? seekTo = MediaInfoNativeMethods.GetBufferSeekRequest(handle);
                if (seekTo is long offset)
                {
                    if (offset < 0 || offset >= stream.Length)
                        return false; // nothing left to read there
                    stream.Seek(offset, SeekOrigin.Begin);
                    MediaInfoNativeMethods.InitializeBuffer(handle, stream.Length, offset);
                }
                return true;
            }

            public void Finish()
            {
                if (!MediaInfoNativeMethods.FinalizeBuffer(handle))
                {
                    throw new MediaInfoNativeFinalizeException("Unable to finalize the stream.");
                }
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (handle != null && !handle.IsInvalid)
                {
                    Close();
                    handle.Dispose();
                }
                disposed = true;
            }
        }
    }
}
