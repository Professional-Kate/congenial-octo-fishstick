using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Assertion.Interface
{
    public interface ICanUnlockAssertion<TID, TCommand> where TCommand : struct
    {
        public void AssertCanUnlock(byte passedLevel, byte requiredLevel, LevelRequirementComponent<TID, TCommand> levelRequirementComponent);
    }
}