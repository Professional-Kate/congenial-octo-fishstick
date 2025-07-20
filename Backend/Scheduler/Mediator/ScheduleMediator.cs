using IdelPog.Common.Repository;
using IdelPog.Common.Structures;

namespace Scheduler.Mediator
{
    public class ScheduleMediator : IScheduleMediator
    {
        private readonly IAssetRepository<Type, IRunnable> _runnableRepository;

        public ScheduleMediator(IAssetRepository<Type, IRunnable> runnableRepository)
        {
            _runnableRepository = runnableRepository;
        }

        public void RunUpdate()
        {
            throw new NotImplementedException();
        }
    }
}