using IdelPog.Common.Structures;

namespace Scheduler.Register
{
    public interface IScheduleReader
    {
        public IReadOnlyList<IScheduledTask> GetScheduledTasks();
    }
}