using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Assertion
{
    public sealed class QueueAssertion<TID, TCommand> : IQueueAssertion<TID, TCommand> where TCommand : struct
    {
        public void AssertSuccessfulDequeue(bool successfulDequeue, LevelRequirementComponent<TID, TCommand> levelRequirementComponent)
        {
            if (successfulDequeue == false)
            {
                throw new UnsuccessfulDequeueException<TID, TCommand>(levelRequirementComponent);
            }
        }
    }
}