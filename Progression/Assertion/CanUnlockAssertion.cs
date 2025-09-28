using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Assertion
{
    public sealed class CanUnlockAssertion<TID, TCommand> : BaseAssertion, ICanUnlockAssertion<TID, TCommand> where TCommand : struct
    {
        public CanUnlockAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertCanUnlock(byte passedLevel, byte requiredLevel, NodeLevelRequirement<TID, TCommand> nodeLevelRequirement)
        {
            Assert<CannotUnlockException<TID, TCommand>>(() =>
            {
                if (passedLevel < requiredLevel)
                {
                    throw new CannotUnlockException<TID, TCommand>(passedLevel, requiredLevel, nodeLevelRequirement);
                }
            });
        }
    }
}