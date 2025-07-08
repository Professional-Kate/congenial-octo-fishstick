using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Assertions.Pipelines
{
    /// <inheritdoc cref="ILevelableAsserter"/>
    public class LevelableAsserter(IAssertUnderMaxLevel assertUnderMaxLevel, IAssertNotNull assertNotNull, IAssertPositive assertPositive)
        : ILevelableAsserter
    {
        public void AssertLevelable(ILevelable levelable)
        {
            assertNotNull.AssertObjectNotNull(levelable);
            assertUnderMaxLevel.AssertLevelIsUnderMax(levelable);
            assertPositive.AssertNumberIsPositive<ILevelable>(levelable.ExperiencePerAction);
        }
    }
}