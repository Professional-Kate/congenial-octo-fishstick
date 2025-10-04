using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Progression.Assertion
{
    public sealed class LevelAssertion : BaseAssertion, ILevelAssertion
    {
        public LevelAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNotAboveMaxLevel(Levelable levelable)
        {
            Assert<MaxLevelException>(() =>
            {
                if (levelable.Level > LevelConstants.MAX_LEVEL)
                {
                    throw new MaxLevelException(levelable, nameof(levelable));
                }
            });
        }

        public void AssertBelowMaxLevel(Levelable levelable)
        {
            Assert<MaxLevelException>(() =>
            {
                if (levelable.Level >= LevelConstants.MAX_LEVEL)
                {
                    throw new MaxLevelException(levelable, nameof(levelable));
                }
            });
        }
    }
}