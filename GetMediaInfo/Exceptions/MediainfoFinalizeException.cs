using System;

namespace GetMediaInfo.Exceptions
{
    /// <summary>
    /// Mediainfo finalize exception.
    /// </summary>
    class MediainfoFinalizeException : Exception
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        public MediainfoFinalizeException(string message) : base(message) { }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MediainfoFinalizeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
