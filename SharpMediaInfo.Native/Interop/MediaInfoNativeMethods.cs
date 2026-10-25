using SharpMediaInfo.Native.Enum;
using System;
using System.Runtime.InteropServices;

namespace SharpMediaInfo.Native.Interop
{
    /// <summary>
    /// Holds all native MediaInfo entry points (generated with <see cref="LibraryImportAttribute"/>)
    /// and the small wrappers that hide the platform string differences.
    /// </summary>
    /// <remarks>
    /// MediaInfo exposes two parallel C APIs: the wide <c>MediaInfo_*</c> functions use the
    /// platform <c>wchar_t</c> (UTF-16 on Windows, but UTF-32 on Linux/macOS) and the narrow
    /// <c>MediaInfoA_*</c> functions use 8-bit/UTF-8 strings. Marshalling UTF-16 on a 4-byte
    /// <c>wchar_t</c> platform corrupts every string, so on non-Windows we use the narrow API
    /// with UTF-8 marshalling, exactly as MediaArea's own wrapper does.
    /// </remarks>
    internal static partial class MediaInfoNativeMethods
    {
        // Logical library name used by the imports. Resolved at runtime via SetDllImportResolver.
        private const string MEDIAINFO_LIB_FILENAME = "MediaInfo";

        // On Windows wchar_t is 2 bytes (UTF-16); elsewhere it is 4 bytes, so the wide API cannot
        // be marshalled as UTF-16 and we route through the narrow (UTF-8) MediaInfoA_* functions.
        private static readonly bool UseAnsi = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        static MediaInfoNativeMethods()
        {
            // Resolve the native library path per OS at runtime
            NativeLibrary.SetDllImportResolver(typeof(MediaInfoNativeMethods).Assembly, (libraryName, assembly, searchPath) =>
            {
                if (!string.Equals(libraryName, MEDIAINFO_LIB_FILENAME, StringComparison.OrdinalIgnoreCase))
                    return IntPtr.Zero;

                string libName;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    libName = "mediainfo.dll";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    libName = "libmediainfo.so";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    libName = "libmediainfo.dylib";
                else
                    return IntPtr.Zero;

                var baseDir = AppContext.BaseDirectory;
                string rid = GetRid();

#if DEBUG
                Console.WriteLine($"[MediaInfo] BaseDir: {baseDir}");
                Console.WriteLine($"[MediaInfo] RID: {rid}");
                Console.WriteLine($"[MediaInfo] Looking for: {libName}");
#endif

                var searchPaths = new[]
                {
                    System.IO.Path.Combine(baseDir, "runtimes", rid, "native"),
                    baseDir
                };

                foreach (var dir in searchPaths)
                {
                    var path = System.IO.Path.Combine(dir, libName);
#if DEBUG
                    Console.WriteLine($"[MediaInfo] Checking: {path} (exists: {System.IO.File.Exists(path)})");
#endif
                    if (System.IO.File.Exists(path) && NativeLibrary.TryLoad(path, out var handle))
                    {
#if DEBUG
                        Console.WriteLine($"[MediaInfo] Loaded successfully from: {path}");
#endif
                        return handle;
                    }
                }

                // Let default resolution try (e.g., system PATH, LD_LIBRARY_PATH)
                return IntPtr.Zero;

                static string GetRid()
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (RuntimeInformation.ProcessArchitecture == Architecture.X86) return "win-x86";
                        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) return "win-arm64";
                        return "win-x64";
                    }
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) return "linux-arm64";
                        return "linux-x64";
                    }
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64) return "osx-arm64";
                        return "osx-x64";
                    }
                    return string.Empty;
                }
            });
        }

        // Marshal a native string pointer back using the encoding that matches the API in use.
        private static string PtrToString(IntPtr value) =>
            (UseAnsi ? Marshal.PtrToStringUTF8(value) : Marshal.PtrToStringUni(value)) ?? string.Empty;

        #region Native imports (size_t parameters are modelled as nint/nuint)

        // --- Handle management (no strings) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_New")]
        private static partial IntPtr New();

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Delete")]
        private static partial void Delete(IntPtr handle);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Close")]
        private static partial void CloseNative(IntPtr handle);

        // --- Open file (string) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open", StringMarshalling = StringMarshalling.Utf16)]
        private static partial nint OpenW(IntPtr handle, string fileName);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfoA_Open", StringMarshalling = StringMarshalling.Utf8)]
        private static partial nint OpenA(IntPtr handle, string fileName);

        // --- Buffer (raw bytes, no strings) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open_Buffer_Init")]
        private static partial nint OpenBufferInit(IntPtr handle, long fileSize, long fileOffset);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open_Buffer_Continue")]
        private static partial nint OpenBufferContinue(IntPtr handle, IntPtr buffer, nuint bufferSize);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open_Buffer_Finalize")]
        private static partial nint OpenBufferFinalize(IntPtr handle);

        // --- Counts ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Count_Get")]
        private static partial nint CountGet(IntPtr handle, nuint streamKind, nint streamNumber);

        // --- Option (string in / string out) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Option", StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr OptionW(IntPtr handle, string option, string value);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfoA_Option", StringMarshalling = StringMarshalling.Utf8)]
        private static partial IntPtr OptionA(IntPtr handle, string option, string value);

        // --- Get by parameter name (string in / string out) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Get", StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr GetW(IntPtr handle, nuint streamKind, nuint streamNumber, string parameter, nuint kindOfInfo, nuint kindOfSearch);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfoA_Get", StringMarshalling = StringMarshalling.Utf8)]
        private static partial IntPtr GetA(IntPtr handle, nuint streamKind, nuint streamNumber, string parameter, nuint kindOfInfo, nuint kindOfSearch);

        // --- Get by parameter index (string out only) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_GetI")]
        private static partial IntPtr GetIW(IntPtr handle, nuint streamKind, nuint streamNumber, nuint parameter, nuint kindOfInfo);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfoA_GetI")]
        private static partial IntPtr GetIA(IntPtr handle, nuint streamKind, nuint streamNumber, nuint parameter, nuint kindOfInfo);

        // --- Inform (string out only) ---
        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Inform")]
        private static partial IntPtr InformW(IntPtr handle, nuint reserved);

        [LibraryImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfoA_Inform")]
        private static partial IntPtr InformA(IntPtr handle, nuint reserved);

        #endregion

        #region Managed wrappers (platform-aware)

        /// <summary>Creates a new MediaInfo handle.</summary>
        public static IntPtr CreateHandle() => New();

        /// <summary>Deletes a MediaInfo handle.</summary>
        public static void DeleteHandle(IntPtr handle) => Delete(handle);

        /// <summary>Closes the currently open file.</summary>
        public static void CloseFile(IntPtr handle) => CloseNative(handle);

        /// <summary>Opens a file by path. Returns true if the file was accepted.</summary>
        public static bool OpenFile(IntPtr handle, string fileName) =>
            (UseAnsi ? OpenA(handle, fileName) : OpenW(handle, fileName)) != 0;

        /// <summary>Prepares the library to accept a buffer.</summary>
        public static bool InitializeBuffer(IntPtr handle, long fileSize, long fileOffset) =>
            OpenBufferInit(handle, fileSize, fileOffset) != 0;

        /// <summary>Feeds a buffer to the library. Returns the status bitfield.</summary>
        public static nint ReadBuffer(IntPtr handle, IntPtr buffer, nuint bufferSize) =>
            OpenBufferContinue(handle, buffer, bufferSize);

        /// <summary>Finalizes a buffered stream. Returns true on success.</summary>
        public static bool FinalizeBuffer(IntPtr handle) => OpenBufferFinalize(handle) != 0;

        /// <summary>Gets the stream count for a kind (pass -1 as streamNumber for the kind total).</summary>
        public static int GetStreamCount(IntPtr handle, StreamKind streamKind, int streamNumber) =>
            (int)CountGet(handle, (nuint)(uint)streamKind, streamNumber);

        /// <summary>Configures the library / reads library information; returns the result string.</summary>
        /// <remarks>
        /// Calls without a handle share one set of native conversion/result buffers, so concurrent
        /// static calls would overwrite each other's option name or result; they are serialized.
        /// </remarks>
        public static string SetOption(IntPtr handle, string option, string value)
        {
            if (handle != IntPtr.Zero)
                return PtrToString(UseAnsi ? OptionA(handle, option, value) : OptionW(handle, option, value));

            lock (StaticOptionLock)
                return PtrToString(UseAnsi ? OptionA(handle, option, value) : OptionW(handle, option, value));
        }

        private static readonly object StaticOptionLock = new();

        /// <summary>Gets a parameter value by name.</summary>
        public static string GetParameter(IntPtr handle, StreamKind streamKind, uint streamNumber,
            string parameter, InfoKind kindOfInfo, InfoKind kindOfSearch) =>
            PtrToString(UseAnsi
                ? GetA(handle, (nuint)(uint)streamKind, streamNumber, parameter, (nuint)(uint)kindOfInfo, (nuint)(uint)kindOfSearch)
                : GetW(handle, (nuint)(uint)streamKind, streamNumber, parameter, (nuint)(uint)kindOfInfo, (nuint)(uint)kindOfSearch));

        /// <summary>Gets a parameter value by index.</summary>
        public static string GetParameter(IntPtr handle, StreamKind streamKind, uint streamNumber,
            uint parameter, InfoKind kindOfInfo) =>
            PtrToString(UseAnsi
                ? GetIA(handle, (nuint)(uint)streamKind, streamNumber, parameter, (nuint)(uint)kindOfInfo)
                : GetIW(handle, (nuint)(uint)streamKind, streamNumber, parameter, (nuint)(uint)kindOfInfo));

        /// <summary>Gets the full report (standard or complete, per the current options).</summary>
        public static string Inform(IntPtr handle) =>
            PtrToString(UseAnsi ? InformA(handle, 0) : InformW(handle, 0));

        #endregion
    }
}
