using IdelPog.Common.Structures;

namespace Scheduler.Tests
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