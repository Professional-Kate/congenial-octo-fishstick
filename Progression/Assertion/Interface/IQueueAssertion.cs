using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Assertion.Interface
{
    public interface IQueueAssertion<TID, TCommand> where TCommand : struct
    {
        public void AssertSuccessfulDequeue(bool successfulDequeue, LevelRequirementComponent<TID, TCommand> levelRequirementComponent);
    }
}