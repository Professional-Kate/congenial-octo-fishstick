using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.SimulationEngine.Assertions
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