using System;
using System.Diagnostics;

namespace GetMediaInfoGUI.Interop
{
    /// <summary>
    /// Process lock to syncronize multiple instances of the application
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class ProcessLock : IDisposable
    {
        // the name of the global mutex;
        /// <summary>
        /// The mutex name
        /// </summary>
        private const string MutexName = "getmediainfo_mutex_variable";

        /// <summary>
        /// The global mutex
        /// </summary>
        private System.Threading.Mutex _globalMutex;

        /// <summary>
        /// The owned
        /// </summary>
        private bool _owned = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLock"/> class.
        /// </summary>
        /// <param name="timeToWait">The time to wait.</param>
        public ProcessLock(TimeSpan timeToWait)
        {
            try
            {
                _globalMutex = new System.Threading.Mutex(true, MutexName, out _owned);

                while (_owned == false)
                {
                    try
                    {
                        _owned = _globalMutex.WaitOne(timeToWait);
                    }
                    catch (System.Threading.AbandonedMutexException)
                    {
                        _owned = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (_owned)
            {
                _globalMutex.ReleaseMutex();
            }
            _globalMutex = null;
        }
    }
}
