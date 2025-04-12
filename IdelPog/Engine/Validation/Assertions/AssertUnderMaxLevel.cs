using IdelPog.Engine.Constants;
using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Validation.Assertions.Handlers;
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