using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Scheduler.Error;
using IdelPog.Core.Scheduler.Error.Factory;
using IdelPog.Core.Scheduler.Registry;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Scheduler.Runner
{
    public class ScheduleRunner : IScheduleRunner
    {
        private readonly IScheduleReader _scheduleReader;
        private readonly IDispatchOne<ScheduledTaskError> _taskErrorDispatcher;
        private readonly ITaskErrorFactory _taskErrorFactory;
        private readonly ICollectionAssertion _collectionAssertion;

        public ScheduleRunner(IScheduleReader scheduleReader, IDispatchOne<ScheduledTaskError> taskErrorDispatcher, ITaskErrorFactory taskErrorFactory, ICollectionAssertion collectionAssertion)
        {
            _scheduleReader = scheduleReader;
            _taskErrorDispatcher = taskErrorDispatcher;
            _taskErrorFactory = taskErrorFactory;
            _collectionAssertion = collectionAssertion;
        }

        public void RunUpdate()
        {
            IScheduledTask[] tasks = _scheduleReader.GetScheduledTasks();
            _collectionAssertion.AssertHasElements(tasks);

            foreach (IScheduledTask task in tasks)
            {
                try
                {
                    task.Run();
                }
                catch (Exception exception)
                {
                    _taskErrorDispatcher.Dispatch(_taskErrorFactory.Create(exception, task.GetType()));
                }
            }
        }
    }
}