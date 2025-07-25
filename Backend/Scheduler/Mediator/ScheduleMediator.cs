using IdelPog.Common.DTO;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch;
using IdelPog.Validation.Assertions;
using Scheduler.Register;
using Scheduler.Types;

namespace Scheduler.Mediator
{
    public class ScheduleMediator : IScheduleMediator
    {
        private readonly IScheduleReader _scheduleReader;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;
        private readonly IDispatchOne<ScheduledTaskErrorDTO> _taskErrorDispatcher;

        public ScheduleMediator(IScheduleReader scheduleReader, IDispatchOne<ScheduledTaskErrorDTO> taskErrorDispatcher, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _scheduleReader = scheduleReader;
            _taskErrorDispatcher = taskErrorDispatcher;
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
                    _taskErrorDispatcher.Dispatch(new  ScheduledTaskErrorDTO { ErrorDTO = new ErrorDTO { Exception = exception }, TaskType = task.GetType()});
                }
            }
        }
    }
}