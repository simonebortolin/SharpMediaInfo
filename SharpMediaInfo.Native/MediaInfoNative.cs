using SharpMediaInfo.Exception;
using SharpMediaInfo.Native.Enum;
using SharpMediaInfo.Native.Exception;
using SharpMediaInfo.Native.Interop;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpMediaInfo.Native
{
    /// <summary>
    /// Initializes a new instance of the MediaInfoNative class using the specified handle and an option to parse
    /// cover data.
    /// </summary>
    /// <param name="handle">The handle to the native MediaInfo resource to be used by this instance. Must be a valid, initialized
    /// handle.</param>
    /// <param name="parseCoverData">true to enable parsing of embedded cover image data; otherwise, false. The default is false.</param>
    internal sealed class MediaInfoNative(bool parseCoverData = false) : IMediaInfoNative
    {

        private MediaInfoHandle? handle;
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
            var ptr = MediaInfoNativeMethods.GetParameter(
                handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(),
                kind,
                (uint)trackIndex,
                parameterName,
                InfoKind.Text,
                InfoKind.Name);

            return ParseOutput(ptr);
        }

        /// <summary>
        /// Gets the value of a property of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <param name="informationKind">the information kind</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        public string GetSpecificParameter(
            StreamKind kind,
            int trackIndex,
            int parameterIndex,
            InfoKind infoKind)
        {
            var ptr = MediaInfoNativeMethods.GetParameter(
                handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(),
                kind,
                (uint)trackIndex,
                (uint)parameterIndex,
                infoKind);

            return ParseOutput(ptr);
        }

        /// <summary>
        /// Configures whether cover image data should be parsed as base64-encoded data.
        /// </summary>
        /// <remarks>Call this method before retrieving cover image data to specify the desired parsing
        /// format. This setting affects how cover data is returned by subsequent operations.</remarks>
        /// <param name="flag">true to enable parsing of cover data as base64; false to disable parsing.</param>
        public void SetCoverDataParsing(bool flag)
        {
            if (parseCoverData)
            {
                MediaInfoNativeMethods.SetOption(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(), "Cover_Data", flag ? "base64" : "");
            }
        }

        /// <summary>
        /// Parses the mediainfo library output. The reading is performed using a buffer of 8192 chars.
        /// </summary>
        /// <param name="pointer">The pointer to the memory area when the informations are stored.</param>
        /// <returns>The parsed string from the memory area referred to the input pointer.</returns>
        private unsafe string ParseOutput(IntPtr pointer)
        {
            var sb = new StringBuilder();

            int len = 8192;
            int index = 0;
            char* ptr = (char*)pointer.ToPointer();
            char[] buffer = new char[len];

            while (*ptr != '\0')
            {
                if (index == len)
                {
                    sb.Append(buffer);
                    index = 0;
                }
                buffer[index] = *ptr;
                ++ptr;
                index++;
            }

            buffer[index] = '\0';
            sb.Append(buffer, 0, index);

            return sb.ToString();
        }

        /// <summary>
        /// Lists media info capacities.
        /// </summary>
        /// <returns>The string that represents the formatted list of Mediainfo library capacities.</returns>
        public static string? Capacities()
        {
            return Marshal.PtrToStringUni(MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Capacities", ""));
        }

        /// <summary>
        /// Lists all info parameters of the MediaInfo library.
        /// </summary>
        /// <returns>The string that represents the formatted list of all available parameters.</returns>
        public static string? ParametersList(bool csvFormat = false)
        {
            return Marshal.PtrToStringUni(MediaInfoNativeMethods.SetOption(IntPtr.Zero, csvFormat ? "Info_Parameters_CSV" : "Info_Parameters", ""));
        }

        /// <summary>
        /// Lists all supported codecs.
        /// </summary>
        /// <returns>The string that represents the formatted list of all supported codecs</returns>
        public static string? KnownCodecs()
        {
            return Marshal.PtrToStringUni(MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Codecs", ""));
        }

        /// <summary>
        /// MediaInfo library version.
        /// </summary>
        /// <returns>The library version</returns>
        public static string? LibraryVersion()
        {
            return Marshal.PtrToStringUni(MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Version", ""));
        }

        /// <summary>
        /// MediaInfo library url.
        /// </summary>
        /// <returns>The library url</returns>
        public static string? LibraryUrl()
        {
            return Marshal.PtrToStringUni(MediaInfoNativeMethods.SetOption(IntPtr.Zero, "Info_Url", ""));
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
            var @string = Marshal.PtrToStringUni(MediaInfoNativeMethods.Inform(handle.DangerousGetHandle(), 0));
            SetCoverDataParsing(true);
            return @string;
        }

        /// <summary>
        /// Gets the standard report.
        /// </summary>
        public string? GetStandardReport()
        {
            MediaInfoNativeMethods.SetOption(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException(), "Complete", "");
            SetCoverDataParsing(false);
            var @string = Marshal.PtrToStringUni(MediaInfoNativeMethods.Inform(handle.DangerousGetHandle(), 0));
            SetCoverDataParsing(true);
            return @string;
        }

        public void Close()
        {
            MediaInfoNativeMethods.CloseFile(handle?.DangerousGetHandle() ?? throw new MediaInfoNativeNotOpenException());
        }

        public void OpenFile(string path)
        {
            handle = new MediaInfoHandle();

            //opens the handle
            if (!MediaInfoNativeMethods.OpenFile(handle.DangerousGetHandle(), path))
            {
                throw new MediaInfoNativeOpenException("Unable to open the file.");
            }
        }

        public void OpenStream(Stream stream)
        {
            if (!stream.CanRead || !stream.CanSeek || stream.Length <= 0)
            {
                throw new ArgumentException("The stream cannot be read or it's empty");
            }

            //get data from stream
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                var rawDataHandle = GCHandle.Alloc(memoryStream.GetBuffer(), GCHandleType.Pinned);
                IntPtr pointer = rawDataHandle.AddrOfPinnedObject();

                //creates a new handle
                handle = new MediaInfoHandle();

                //opens the buffer
                if (!MediaInfoNativeMethods.InitializeBuffer(handle.DangerousGetHandle(), stream.Length, 0))
                {
                    throw new MediaInfoNativeOpenException("Unable to open the stream.");
                }
                MediaInfoNativeMethods.ReadBuffer(handle.DangerousGetHandle(), pointer, (UIntPtr)memoryStream.Length);
                if (!MediaInfoNativeMethods.FinalizeBuffer(handle.DangerousGetHandle()))
                {
                    throw new MediaInfoNativeFinalizeException("Unable to finalize the stream.");
                }
            }
        }
    }
}
