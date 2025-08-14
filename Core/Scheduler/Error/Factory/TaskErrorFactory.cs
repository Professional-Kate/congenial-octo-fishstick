using IdelPog.Core.Factory.Interface;

namespace IdelPog.Core.Scheduler.Error.Factory
{
    public class TaskErrorFactory : ITaskErrorFactory
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public TaskErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public ScheduledTaskError Create(Exception exception, Type taskType)
        {
            return new ScheduledTaskError
            {
                BaseError = _baseErrorFactory.Create(exception),
                TaskType = taskType
            };
        }
    }
}