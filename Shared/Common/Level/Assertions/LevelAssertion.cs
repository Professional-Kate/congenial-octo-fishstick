using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Common.Level.Assertions
{
    public class LevelAssertion : BaseAssertion, ILevelAssertion
    {
        public LevelAssertion(IHandler handler) : base(handler)
        {
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