using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Assertion.Interface
{
    public interface ICanUnlockAssertion<TCommand> where TCommand : struct
    {
        public void AssertCanUnlock(byte passedLevel, byte requiredLevel, NodeLevelRequirement<TCommand> nodeLevelRequirement);
    }
}