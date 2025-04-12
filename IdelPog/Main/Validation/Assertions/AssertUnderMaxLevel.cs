using IdelPog.Main.Constants;
using IdelPog.Main.Structures.Models.Levelable;
using IdelPog.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Main.Validation.Assertions.Interfaces;
using IdelPog.Main.Validation.Exceptions;

namespace IdelPog.Main.Validation.Assertions
{
    public class AssertUnderMaxLevel : BaseAssertion<MaxLevelException>, IAssertUnderMaxLevel
    {
        public AssertUnderMaxLevel(IHandler handler) : base(handler) { }


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