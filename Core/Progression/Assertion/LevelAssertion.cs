using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Progression.Assertion
{
    public sealed class LevelAssertion : ILevelAssertion
    {

        public void AssertNotAboveMaxLevel(Levelable levelable)
        {
            if (levelable.Level > LevelConstants.MAX_LEVEL)
            {
                throw new MaxLevelException(levelable, nameof(levelable));
            }
        }

        public void AssertBelowMaxLevel(Levelable levelable)
        {
            if (levelable.Level >= LevelConstants.MAX_LEVEL)
            {
                throw new MaxLevelException(levelable, nameof(levelable));
            }
        }
    }
}