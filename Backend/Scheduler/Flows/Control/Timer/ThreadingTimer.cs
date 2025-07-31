using Scheduler.Core.Mediator;

namespace Scheduler.Flows.Control
{
    public sealed class ThreadingTimer : IManagedTimer
    {
        private readonly IScheduleRunnerMediator _scheduleRunnerMediator;
        
        private readonly Timer _timer;
        private bool _disposed;

        public ThreadingTimer(IScheduleRunnerMediator scheduleRunnerMediator)
        {
            _scheduleRunnerMediator = scheduleRunnerMediator;
            _timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Callback(object? state)
        {
            _scheduleRunnerMediator.RunUpdate();
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