using System;

namespace GetMediaInfo.Exception
{
    /// <summary>
    /// Mediainfo open exception.
    /// </summary>
    public class MediainfoOpenException : System.Exception
    {
        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        public MediainfoOpenException(string message) : base(message) { }

        /// <summary>
        /// Secondary constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public MediainfoOpenException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
