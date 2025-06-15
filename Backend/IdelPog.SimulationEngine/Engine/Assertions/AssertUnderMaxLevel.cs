using IdelPog.Engine.Constants;
using IdelPog.Engine.Models;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Engine.Assertions
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