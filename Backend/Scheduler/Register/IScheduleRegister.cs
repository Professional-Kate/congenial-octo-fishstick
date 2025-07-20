using IdelPog.Common.Structures;

namespace Scheduler.Register
{
    public interface IScheduleRegister
    {
        public ReadOnlySpan<IRunnable> GetRunnables();

        public void Register(IRunnable runnable);
        
        public void Unregister(IRunnable runnable);
    }
}