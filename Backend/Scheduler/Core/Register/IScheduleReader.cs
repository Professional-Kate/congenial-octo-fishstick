using IdelPog.Common.Structures;

namespace Scheduler.Core.Register
{
    public interface IScheduleReader
    {
        public IReadOnlyList<IScheduledTask> GetScheduledTasks();
    }
}