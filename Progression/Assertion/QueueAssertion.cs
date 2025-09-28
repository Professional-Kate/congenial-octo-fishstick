using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Assertion
{
    public sealed class QueueAssertion<TCommand> : BaseAssertion, IQueueAssertion<TCommand> where TCommand : struct
    {
        public QueueAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertSuccessfulDequeue(bool successfulDequeue, NodeLevelRequirement<TCommand> nodeLevelRequirement)
        {
            Assert<UnsuccessfulDequeueException<TCommand>>(() =>
            {
                if (successfulDequeue == false)
                {
                    throw new UnsuccessfulDequeueException<TCommand>(nodeLevelRequirement);
                }
            });
        }
    }
}