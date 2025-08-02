using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Validation.Assertions;
using Scheduler.Core.Register;
using Scheduler.Factory;
using Scheduler.Types;

namespace Scheduler.Core.Mediator
{
    public class ScheduleRunnerMediator : IScheduleRunnerMediator
    {
        private readonly IScheduleReader _scheduleReader;
        private readonly IDispatchOne<ScheduledTaskErrorDTO> _taskErrorDispatcher;
        private readonly ITaskErrorDTOFactory _taskErrorDTOFactory;
        private readonly ICollectionAssertion _collectionAssertion;

        public ScheduleRunnerMediator(IScheduleReader scheduleReader, IDispatchOne<ScheduledTaskErrorDTO> taskErrorDispatcher, ITaskErrorDTOFactory taskErrorDTOFactory,
            ICollectionAssertion collectionAssertion)
        {
            _scheduleReader = scheduleReader;
            _taskErrorDispatcher = taskErrorDispatcher;
            _taskErrorDTOFactory = taskErrorDTOFactory;
            _collectionAssertion = collectionAssertion;
        }

        public void RunUpdate()
        {
            IReadOnlyList<IScheduledTask> tasks = _scheduleReader.GetScheduledTasks();
            _collectionAssertion.AssertNotEmpty(tasks);

            foreach (IScheduledTask task in tasks)
            {
                try
                {
                    task.Run();
                }
                catch (Exception exception)
                {
                    _taskErrorDispatcher.Dispatch(_taskErrorDTOFactory.Create(exception, task.GetType()));
                }
            }
        }
    }
}