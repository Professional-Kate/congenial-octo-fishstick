using Scheduler.Mediator;

namespace Scheduler.Runner
{
    public class ScheduleRunner : IScheduleRunner
    {
        private readonly Timer _timer;

        public ScheduleRunner(IScheduleMediator scheduleMediator)
        {
            _timer = new Timer(_ => scheduleMediator.RunUpdate(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void StartSchedule(TimeSpan interval)
        {
            _timer.Change(interval, interval);
        }

        public void StopSchedule()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
    }
}