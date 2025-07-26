using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Assertions.Pipelines
{
    /// <inheritdoc cref="ILevelableAssertionPipeline"/>
    public class LevelableAssertionPipeline(ILevelAssertion levelAssertion, IAssertNotNull assertNotNull, INumberAssertion numberAssertion)
        : ILevelableAssertionPipeline
    {
        public void AssertLevelable(ILevelable levelable)
        {
            assertNotNull.AssertObjectNotNull(levelable);
            levelAssertion.AssertBelowMaxLevel(levelable);
            numberAssertion.AssertNonNegative(levelable.ExperiencePerAction);
        }
    }
}