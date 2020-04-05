using System;

namespace GetMediaInfo.Exceptions
{
    /// <summary>
    /// Mediainfo analysis exception.
    /// </summary>
    public class MediaInfoAnalysisException : Exception
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        public MediaInfoAnalysisException(string message) : base(message) { }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MediaInfoAnalysisException(string message, Exception innerException) : base(message, innerException) { }
    }
}
