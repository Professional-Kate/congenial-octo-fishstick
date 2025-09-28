using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Assertion
{
    public sealed class QueueAssertion<TID, TCommand> : BaseAssertion, IQueueAssertion<TID, TCommand> where TCommand : struct
    {
        public QueueAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertSuccessfulDequeue(bool successfulDequeue, LevelRequirementComponent<TID, TCommand> levelRequirementComponent)
        {
            Assert<UnsuccessfulDequeueException<TID, TCommand>>(() =>
            {
                if (successfulDequeue == false)
                {
                    throw new UnsuccessfulDequeueException<TID, TCommand>(levelRequirementComponent);
                }
            });
        }
    }
}