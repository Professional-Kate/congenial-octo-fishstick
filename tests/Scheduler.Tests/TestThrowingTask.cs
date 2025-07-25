using IdelPog.Common.Structures;

namespace Scheduler.Tests
{
    internal class TestThrowingTask : IScheduledTask
    {
        public int AmountCalled { get; private set; }

        public void Run()
        {
            AmountCalled++;
            throw new Exception();
        }
    }
}