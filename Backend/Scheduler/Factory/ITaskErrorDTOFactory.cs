using Scheduler.Types;

namespace Scheduler.Factory
{
    public interface ITaskErrorDTOFactory
    {
        public ScheduledTaskErrorDTO Create(Exception exception, Type taskType);
    }
}