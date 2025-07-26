namespace Scheduler.Core
{
    public sealed class ThreadingTimer : IManagedTimer
    {
        private readonly Timer _timer;
        private bool _disposed;

        public ThreadingTimer(Action callback)
        {
            _timer = new Timer(_ => callback(), null, Timeout.Infinite, Timeout.Infinite);
        }
        
        ~ThreadingTimer()
        {
            Dispose(false);
        }

        public void Start(TimeSpan dueTime, TimeSpan period)
        {
            _timer.Change(dueTime, period);
        }

        public void Stop()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // CA1816
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _timer.Dispose();
            }
            
            _disposed = true;
        }
    }
}