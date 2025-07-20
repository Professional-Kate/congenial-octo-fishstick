using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;
using Scheduler.Register;

namespace Scheduler.Mediator
{
    public class ScheduleMediator : IScheduleMediator
    {
        private readonly IScheduleRegister  _scheduleRegister;
        private readonly IAssertCollectionNotEmpty  _assertCollectionNotEmpty;

        public ScheduleMediator(IScheduleRegister scheduleRegister, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _scheduleRegister = scheduleRegister;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }

        public void RunUpdate()
        {
            ReadOnlySpan<IRunnable> runnables = _scheduleRegister.GetRunnables();
            _assertCollectionNotEmpty.Handle(runnables);
            
            foreach (IRunnable runnable in runnables)
            {
                runnable.Run();
            }
        }
    }
}