using IdelPog.Common.Repository;
using IdelPog.Common.Structures;

namespace Scheduler.Register
{
    public class ScheduleRegister : IScheduleRegister
    {
        private readonly IAssetRepository<Type, IRunnable> _runnableRepository;

        public ScheduleRegister(IAssetRepository<Type, IRunnable> runnableRepository)
        {
            _runnableRepository = runnableRepository;
        }

        public ReadOnlySpan<IRunnable> GetRunnables()
        {
            throw new NotImplementedException();
        }

        public void Register(IRunnable runnable)
        {
            throw new NotImplementedException();
        }

        public void Unregister(IRunnable runnable)
        {
            throw new NotImplementedException();
        }
    }
}