using SharpMediaInfo.Native.Enum;
namespace SharpMediaInfo.Native
{
    public interface IMediaInfoNative : IDisposable
    {
        /// <summary>
        /// Gets the value of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns>
        /// A string containing the parameter's value or an empty string in case of problems.
        /// </returns>
        string GetParameter(StreamKind kind, int trackIndex, string parameterName);

        /// <summary>
        /// Gets the value of a property of a parameter of a track at a defined index with a defined kind.
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="trackIndex">The track index</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <param name="informationKind">the information kind</param>
        /// <returns>A string containing the parameter's value or an empty string in case of problems.</returns>
        string GetSpecificParameter(
            StreamKind kind,
            int trackIndex,
            int parameterIndex,
            InfoKind infoKind);

        /// <summary>
        /// Gets stream count for a defined kind.
        /// </summary>
        /// <param name="kind">The defined kind</param>
        /// <returns>The stream count.</returns>
        int GetStreamCount(StreamKind kind);

        /// <summary>
        /// Gets the complete report.
        /// </summary>
        string? GetCompleteReport();

        /// <summary>
        /// Gets the standard report.
        /// </summary>
        string? GetStandardReport();

        /// <summary>
        /// Closes the currently open file and releases any associated resources.
        /// </summary>
        /// <remarks>After calling this method, attempts to read from or write to the file may result in
        /// an exception. This method should be called when file operations are complete to ensure that all resources
        /// are properly released.</remarks>
        void Close();

        /// <summary>
        /// Opens the file at the specified path for reading or writing.
        /// </summary>
        /// <param name="path">The full path to the file to open. Cannot be null or empty.</param>
        void OpenFile(string path);

        /// <summary>
        /// Analyzes a readable, seekable stream. The stream is read in chunks, following the
        /// seek requests of the library, so only the parts MediaInfo needs are read (typically
        /// a few MB, whatever the size of the file) and nothing is buffered in memory.
        /// </summary>
        /// <param name="stream">A readable, seekable, non-empty stream. Cannot be null.</param>
        void OpenStream(Stream stream);

        /// <summary>
        /// Asynchronous variant of <see cref="OpenStream(Stream)"/>: reads go through
        /// <see cref="Stream.ReadAsync(Memory{byte}, CancellationToken)"/>, so the caller's thread is free
        /// while I/O is pending (needed in the browser, where there is a single thread and
        /// file reads are asynchronous).
        /// </summary>
        /// <param name="stream">A readable, seekable, non-empty stream. Cannot be null.</param>
        /// <param name="cancellationToken">Cancels the analysis between two reads.</param>
        Task OpenStreamAsync(Stream stream, CancellationToken cancellationToken = default);

        static IMediaInfoNative Create(bool parseCoverData) => new MediaInfoNative(parseCoverData);

        /// <summary>
        /// Gets library information using MediaInfo_Option with IntPtr.Zero handle.
        /// </summary>
        static string GetLibraryInfo(string option, string value = "")
        {
            return MediaInfoNative.GetLibraryInfoStatic(option, value);
        }
    }
}
