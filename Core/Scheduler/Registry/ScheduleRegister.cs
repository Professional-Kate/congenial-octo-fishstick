using IdelPog.Core.Contracts;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Scheduler.Registry
{
    public sealed class ScheduleRegister : IScheduleRegister, IScheduleReader
    {
        private readonly List<IScheduledTask> _taskList = [];
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public ScheduleRegister(IUniqueAssertion uniqueAssertion, IFoundAssertion foundAssertion, IObjectNullAssertion objectNullAssertion)
        {
            _uniqueAssertion = uniqueAssertion;
            _foundAssertion = foundAssertion;
            _objectNullAssertion = objectNullAssertion;
        }

        public IScheduledTask[] GetScheduledTasks()
        {
            return _taskList.ToArray();
        }

        public void Register(IScheduledTask scheduledTask)
        {
            _objectNullAssertion.AssertNotNull(scheduledTask, nameof(scheduledTask));
            _uniqueAssertion.AssertUnique(scheduledTask, _taskList.Contains(scheduledTask));
            _taskList.Add(scheduledTask);
        }

        public void Unregister(IScheduledTask scheduledTask)
        {
            _objectNullAssertion.AssertNotNull(scheduledTask, nameof(scheduledTask));
            _foundAssertion.AssertFound(scheduledTask, _taskList.Contains(scheduledTask));
            _taskList.Remove(scheduledTask);
        }
    }
}