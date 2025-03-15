using IdelPog.Constants;
using IdelPog.Structures.Models.Levelable;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Validation.Assertions
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