using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Assertion
{
    public sealed class QueueAssertion : IQueueAssertion
    {
        public void AssertSuccessfulDequeue(bool successfulDequeue)
        {
            if (successfulDequeue == false)
            {
                throw new UnsuccessfulDequeueException();
            }
        }

        public void AssertSuccessfulPeek(bool successfulPeek)
        {
            if (successfulPeek == false)
            {
                throw new UnsuccessfulPeekException();
            }
        }
    }
}