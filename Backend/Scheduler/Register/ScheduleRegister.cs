using IdelPog.Common.Repository;
using IdelPog.Common.Structures;

namespace Scheduler.Register
{
    public class ScheduleRegister : IScheduleRegister, IScheduleReader
    {
        private readonly IAssetRepository<Type, IScheduledTask> _runnableRepository;

        public ScheduleRegister(IAssetRepository<Type, IScheduledTask> runnableRepository)
        {
            _runnableRepository = runnableRepository;
        }

        public IReadOnlyList<IScheduledTask> GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        public void Register(IScheduledTask scheduledTask)
        {
            throw new NotImplementedException();
        }

        public void Unregister(IScheduledTask scheduledTask)
        {
            throw new NotImplementedException();
        }
    }
}