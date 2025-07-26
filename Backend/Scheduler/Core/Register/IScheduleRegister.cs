using IdelPog.Common.Structures;

namespace Scheduler.Core.Register
{
    public interface IScheduleRegister
    {
        public void Register(IScheduledTask scheduledTask);

        public void Unregister(IScheduledTask scheduledTask);
    }
}