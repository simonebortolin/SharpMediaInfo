using SharpMediaInfo.Native.Enum;
namespace SharpMediaInfo.Native
{
    public interface IMediaInfoNative
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
        /// Opens the specified stream for reading or writing operations.
        /// </summary>
        /// <param name="stream">The stream to open. The stream must be readable or writable, depending on the intended operation. Cannot be
        /// null.</param>
        void OpenStream(Stream stream);

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
