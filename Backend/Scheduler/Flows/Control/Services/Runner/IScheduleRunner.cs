namespace Scheduler.Flows.Control.Runner
{
    public interface IScheduleRunner
    {
        public void StartSchedule(TimeSpan interval);

        public void StopSchedule();
    }
}