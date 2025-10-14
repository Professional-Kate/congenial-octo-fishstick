using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Assertion
{
    public sealed class CanUnlockAssertion<TID, TCommand> : ICanUnlockAssertion<TID, TCommand> where TCommand : struct
    {
        public void AssertCanUnlock(byte passedLevel, byte requiredLevel, LevelRequirementComponent<TID, TCommand> levelRequirementComponent)
        {
            if (passedLevel < requiredLevel)
            {
                throw new CannotUnlockException<TID, TCommand>(passedLevel, requiredLevel, levelRequirementComponent);
            }
        }
    }
}