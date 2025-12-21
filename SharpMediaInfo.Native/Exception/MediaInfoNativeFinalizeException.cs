using System;

namespace SharpMediaInfo.Exception
{
    /// <summary>
    /// Mediainfo finalize exception.
    /// </summary>
    class MediaInfoNativeFinalizeException : System.Exception
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        public MediaInfoNativeFinalizeException(string message) : base(message) { }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MediaInfoNativeFinalizeException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
