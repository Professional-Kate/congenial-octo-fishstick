using IdelPog.Common.Structures;

namespace Scheduler.Register
{
    public interface IScheduleRegister
    {
        public void Register(IScheduledTask scheduledTask);
        
        public void Unregister(IScheduledTask scheduledTask);
    }
}