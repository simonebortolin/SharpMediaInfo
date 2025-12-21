using System;

namespace SharpMediaInfo.Native.Exception
{
    /// <summary>
    /// Mediainfo open exception.
    /// </summary>
    public class MediaInfoNativeOpenException : System.Exception
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        public MediaInfoNativeOpenException(string message) : base(message) { }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MediaInfoNativeOpenException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
