using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;

namespace Scheduler.Register
{
    public class ScheduleRegister : IScheduleRegister, IScheduleReader
    {
        private readonly IList<IScheduledTask> _taskList = new List<IScheduledTask>();
        private readonly IAssertNonDuplicate _assertNonDuplicate;
        private readonly IAssertFound _assertFound;
        private readonly IAssertNotNull _assertNotNull;

        public ScheduleRegister(IAssertNonDuplicate assertNonDuplicate, IAssertFound assertFound, IAssertNotNull assertNotNull)
        {
            _assertNonDuplicate = assertNonDuplicate;
            _assertFound = assertFound;
            _assertNotNull = assertNotNull;
        }

        public IReadOnlyList<IScheduledTask> GetScheduledTasks()
        {
            return _taskList.AsReadOnly();
        }

        public void Register(IScheduledTask scheduledTask)
        {
            _assertNotNull.AssertObjectNotNull(scheduledTask);
            _assertNonDuplicate.AssertContains(scheduledTask, () => _taskList.Contains(scheduledTask));
            _taskList.Add(scheduledTask);
        }

        public void Unregister(IScheduledTask scheduledTask)
        {
            _assertNotNull.AssertObjectNotNull(scheduledTask);
            _assertFound.AssertItemIsFound(scheduledTask, () => _taskList.Contains(scheduledTask));
            _taskList.Remove(scheduledTask);
        }
    }
}