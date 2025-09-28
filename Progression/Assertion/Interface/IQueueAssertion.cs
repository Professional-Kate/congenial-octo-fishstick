using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Assertion.Interface
{
    public interface IQueueAssertion<TCommand> where TCommand : struct
    {
        public void AssertSuccessfulDequeue(bool successfulDequeue, NodeLevelRequirement<TCommand> nodeLevelRequirement);
    }
}