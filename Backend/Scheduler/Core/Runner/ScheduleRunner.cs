namespace Scheduler.Core.Runner
{
    public class ScheduleRunner : IScheduleRunner
    {
        private readonly IManagedTimer _managedTimer;

        public ScheduleRunner(IManagedTimer managedTimer)
        {
            _managedTimer = managedTimer;
        }

        public void StartSchedule(TimeSpan interval)
        {
            _managedTimer.Start(interval, interval);
        }

        public void StopSchedule()
        {
            _managedTimer.Stop();
        }
    }
}