using IdelPog.Core.Contracts;

namespace IdelPog.Core.Scheduler.Registry
{
    public interface IScheduleRegister
    {
        public void Register(IScheduledTask scheduledTask);

        public void Unregister(IScheduledTask scheduledTask);
    }
}