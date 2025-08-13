namespace IdelPog.Skill.Timer
{
    public sealed class ThreadingTimer : IManagedTimer
    {
        private readonly System.Threading.Timer _timer;
        private bool _disposed;

        public ThreadingTimer()
        {
            _timer = new System.Threading.Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Callback(object? state)
        {
            // TODO: call skill boi
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