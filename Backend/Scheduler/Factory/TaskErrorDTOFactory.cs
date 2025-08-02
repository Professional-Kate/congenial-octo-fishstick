using IdelPog.Common.Factories;
using Scheduler.Types;

namespace Scheduler.Factory
{
    public class TaskErrorDTOFactory : ITaskErrorDTOFactory
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public TaskErrorDTOFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public ScheduledTaskErrorDTO Create(Exception exception, Type taskType)
        {
            return new ScheduledTaskErrorDTO
            {
                BaseError = _baseErrorFactory.Create(exception),
                TaskType = taskType
            };
        }
    }
}