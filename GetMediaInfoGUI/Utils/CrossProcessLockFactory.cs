using System;

namespace GetMediaInfoGUI
{
    /// <summary>
    /// Process Lock factory class
    /// </summary>
    public class CrossProcessLockFactory
    {
        /// <summary>
        /// The default timout in seconds
        /// </summary>
        private static int DefaultTimoutInSeconds = 5;
        /// <summary>
        /// Creates the cross process lock.
        /// </summary>
        /// <returns></returns>
        public static IDisposable CreateCrossProcessLock()
        {
            return new ProcessLock(TimeSpan.FromSeconds(DefaultTimoutInSeconds));
        }

        /// <summary>
        /// Creates the cross process lock.
        /// </summary>
        /// <param name="timespan">The timespan.</param>
        /// <returns></returns>
        public static IDisposable CreateCrossProcessLock(TimeSpan timespan)
        {
            return new ProcessLock(timespan);
        }
    }
}
