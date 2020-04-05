using System;
using System.Diagnostics;
using System.Threading;

namespace GetMediaInfoGUI
{
    public class ProcessLock : IDisposable
    {
        private const string MutexName = "getmediainfo_mutex_variable";
        private Mutex _globalMutex;
        private bool _owned;

        public ProcessLock(TimeSpan timeToWait)
        {
            try
            {
                _globalMutex = new Mutex(true, "getmediainfo_mutex_variable", out _owned);
                while (!_owned)
                {
                    try
                    {
                        _owned = _globalMutex.WaitOne(timeToWait);
                    }
                    catch (AbandonedMutexException ex)
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

        public void Dispose()
        {
            if (_owned)
                _globalMutex.ReleaseMutex();
            _globalMutex = null;
        }
    }
}
