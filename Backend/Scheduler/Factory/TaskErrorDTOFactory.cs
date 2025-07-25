using IdelPog.Common.Factories;
using Scheduler.Types;

namespace Scheduler.Factory
{
    public class TaskErrorDTOFactory : ITaskErrorDTOFactory
    {
        private readonly IErrorDTOFactory _errorDTOFactory;

        public TaskErrorDTOFactory(IErrorDTOFactory errorDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
        }

        public ScheduledTaskErrorDTO Create(Exception exception, Type taskType)
        {
            return new ScheduledTaskErrorDTO
            {
                ErrorDTO = _errorDTOFactory.Create(exception),
                TaskType = taskType
            };
        }
    }
}