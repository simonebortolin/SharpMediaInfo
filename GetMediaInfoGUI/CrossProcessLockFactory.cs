using System;

namespace GetMediaInfoGUI
{
    public class CrossProcessLockFactory
    {
        private static int DefaultTimoutInSeconds = 5;

        public static IDisposable CreateCrossProcessLock()
        {
            return new ProcessLock(TimeSpan.FromSeconds(DefaultTimoutInSeconds));
        }

        public static IDisposable CreateCrossProcessLock(TimeSpan timespan)
        {
            return new ProcessLock(timespan);
        }
    }
}
