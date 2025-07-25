using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch;
using IdelPog.Validation.Assertions;
using Scheduler.Factory;
using Scheduler.Register;
using Scheduler.Types;

namespace Scheduler.Mediator
{
    public class ScheduleMediator : IScheduleMediator
    {
        private readonly IScheduleReader _scheduleReader;
        private readonly IDispatchOne<ScheduledTaskErrorDTO> _taskErrorDispatcher;
        private readonly ITaskErrorDTOFactory  _taskErrorDTOFactory;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public ScheduleMediator(IScheduleReader scheduleReader, IDispatchOne<ScheduledTaskErrorDTO> taskErrorDispatcher, ITaskErrorDTOFactory taskErrorDTOFactory, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _scheduleReader = scheduleReader;
            _taskErrorDispatcher = taskErrorDispatcher;
            _taskErrorDTOFactory = taskErrorDTOFactory;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }

        public void RunUpdate()
        {
            IReadOnlyList<IScheduledTask> tasks = _scheduleReader.GetScheduledTasks();
            _assertCollectionNotEmpty.Handle(tasks);
            
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