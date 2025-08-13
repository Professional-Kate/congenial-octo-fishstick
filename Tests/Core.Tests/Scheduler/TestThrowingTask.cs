using IdelPog.Core.Contracts;

namespace IdelPog.Core.Tests.Scheduler
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