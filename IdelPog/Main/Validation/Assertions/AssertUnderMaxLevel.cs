using IdelPogTemp.Main.Constants;
using IdelPogTemp.Main.Structures.Models.Levelable;
using IdelPogTemp.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Validation.Assertions
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