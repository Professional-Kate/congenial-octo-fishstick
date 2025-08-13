using IdelPog.Core.Contracts;

namespace IdelPog.Core.Tests.Scheduler
{
    internal class TestScheduledTask : IScheduledTask
    {
        public bool WasCalled { get; private set; }
        public int AmountCalled { get; private set; }

        public void Run()
        {
            WasCalled = true;
            AmountCalled++;
        }
    }
}