using System;
using System.Runtime.InteropServices;

namespace GetMediaInfo
{
    /// <summary>
    /// Class that holds all native MediaInfoLib external methods
    /// </summary>
    class MediaInfoLib
    {
        private const string MEDIAINFO_LIB_FILENAME = "MediaInfo.dll";

        #region Handle management
        /// <summary>
        /// Creates a new MediaInfo handle.
        /// </summary>
        /// <returns>The MediaInfo handle</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_New")]
        public static extern IntPtr CreateHandle();

        /// <summary>
        /// Deletes a Mediainfo handle.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        [DllImport("MediaInfo.dll", EntryPoint = "MediaInfo_Delete")]
        internal static extern void DeleteHandle(IntPtr Handle);
        #endregion

        #region Configuration
        /// <summary>
        /// Configures the MediaInfoLib.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="OptionString">The name of the option</param>
        /// <param name="Value">The value of the option</param>
        /// <returns>Empty string, if it fails; otherwise some additional information depending on the configured option</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, CharSet = CharSet.Unicode, EntryPoint = "MediaInfo_Option")]
        public static extern IntPtr SetOption(IntPtr Handle, string OptionString, string Value);
        #endregion

        #region File
        /// <summary>
        /// Opens a file in the specified path.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="FileName">The full name of the file</param>
        /// <returns>True, if the file is correctly opened; otherwise false.</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, CharSet = CharSet.Unicode, EntryPoint = "MediaInfo_Open")]
        public static extern bool OpenFile(IntPtr Handle, string FileName);

        /// <summary>
        /// Close a file.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Close")]
        public static extern void CloseFile(IntPtr Handle);
        #endregion

        #region Buffer
        /// <summary>
        /// Prepares the MediaInfoLib to accept a buffer.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="FileSize">The estimated file size</param>
        /// <param name="FileOffset">The initial offset to seek the beginning of the file (if the file beginning is unknown)</param>
        /// <returns>True, if the operation is successful; otherwise false</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open_Buffer_Init")]
        public static extern bool InitializeBuffer(IntPtr Handle, long FileSize, long FileOffset);

        /// <summary>
        /// Opens a stream.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="Buffer">The pointer of the buffer</param>
        /// <param name="BufferSize">The buffer size</param>
        /// <returns>
        /// A bitfield:
        /// <para />bit 0: Is Accepted (format is known)
        /// <para />bit 1: Is Filled (main data is collected)
        /// <para />bit 2: Is Updated (some data have beed updated, example: duration for a real time MPEG-TS stream)
        /// <para />bit 3: Is Finalized (No more data is needed, will not use further data)
        /// <para />bit 4-15: Reserved
        /// <para />bit 16-31: User defined
        /// </returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open_Buffer_Continue")]
        public static extern IntPtr ReadBuffer(IntPtr Handle, IntPtr Buffer, UIntPtr BufferSize);

        /// <summary>
        /// Closes a stream.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <returns>True, if the stream is correctly closed; otherwise false.</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Open_Buffer_Finalize")]
        public static extern bool FinalizeBuffer(IntPtr Handle);
        #endregion

        #region Information
        /// <summary>
        /// Count of streams of a stream kind (StreamNumber not filled), or count of piece of information in this stream.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="StreamKind">The stream kind</param>
        /// <param name="StreamNumber">The stream index</param>
        /// <returns>The count of fields for this stream kind / stream number if stream number is provided, else the count of streams for this stream kind</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, EntryPoint = "MediaInfo_Count_Get")]
        public static extern int GetStreamCount(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, int StreamNumber);

        /// <summary>
        /// Get a piece of specific information about a file.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="StreamKind">The stream kind</param>
        /// <param name="StreamNumber">The stream index</param>
        /// <param name="Parameter">The parameter identifier</param>
        /// <param name="KindOfInfo">The kind of information about the parameter</param>
        /// <param name="KindOfSearch">Where to look for the parameter</param>
        /// <returns>The pointer to a memory location that contains a string with the requested information that can be empty if an error has occurred</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, CharSet = CharSet.Unicode, EntryPoint = "MediaInfo_Get")]
        public static extern IntPtr GetParameter(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, uint StreamNumber,
            string Parameter, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfInfo, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfSearch);

        /// <summary>
        /// Get a piece of information about a file
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="StreamKind">The stream kind</param>
        /// <param name="StreamNumber">The stream index</param>
        /// <param name="Parameter">The parameter identifier</param>
        /// <param name="KindOfInfo">The kind of information about the parameter</param>
        /// <returns>The pointer to a memory location that contains a string with the requested information that can be empty if an error has occurred.</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, CharSet = CharSet.Unicode, EntryPoint = "MediaInfo_GetI")]
        public static extern IntPtr GetParameter(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] StreamKind StreamKind, uint StreamNumber,
            uint Parameter, [MarshalAs(UnmanagedType.U4)] InfoKind KindOfInfo);

        /// <summary>
        /// Get all details about a file.
        /// </summary>
        /// <param name="Handle">The MediaInfo handle</param>
        /// <param name="Reserved">Reserved parameter (do not use)</param>
        /// <returns>The pointer to a memory location that contains a string with the requested information that can be empty if an error has occurred.</returns>
        [DllImport(MEDIAINFO_LIB_FILENAME, CharSet = CharSet.Unicode, EntryPoint = "MediaInfo_Inform")]
        public static extern IntPtr Inform(IntPtr Handle, [MarshalAs(UnmanagedType.U4)] uint Reserved);
        #endregion
    }
}
