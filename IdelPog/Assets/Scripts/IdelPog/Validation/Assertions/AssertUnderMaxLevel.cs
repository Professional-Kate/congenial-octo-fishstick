using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public class AssertUnderMaxLevel : BaseAssertion, IAssertUnderMaxLevel
    {
        public AssertUnderMaxLevel(IHandler handler) : base(handler) { }


        public void AssertLevelIsUnderMax(Job levelable)
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