using IdelPog.Engine.Constants;
using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Engine.Validation.Assertions.Interfaces;
using IdelPog.Engine.Validation.Exceptions;

namespace IdelPog.Engine.Validation.Assertions
{
    public class AssertUnderMaxLevel(IHandler handler) : BaseAssertion<MaxLevelException>(handler), IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable)
        {
            Assert(() =>
            {
                if (levelable.Level == JobConstants.MAX_JOB_LEVEL)
                {
                    throw new MaxLevelException(levelable);
                }
            });
        }
    }
}