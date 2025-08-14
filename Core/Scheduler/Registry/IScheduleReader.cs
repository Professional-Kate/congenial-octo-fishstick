using IdelPog.Core.Contracts;

namespace IdelPog.Core.Scheduler.Registry
{
    public interface IScheduleReader
    {
        public IScheduledTask[] GetScheduledTasks();
    }
}