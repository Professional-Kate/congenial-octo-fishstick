using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Assertions.Pipelines
{
    /// <inheritdoc cref="ILevelableAssertionPipeline"/>
    public class LevelableAssertionPipeline(ILevelAssertion levelAssertion, IObjectNullAssertion objectNullAssertion, INumberAssertion numberAssertion)
        : ILevelableAssertionPipeline
    {
        public void AssertLevelable(ILevelable levelable)
        {
            objectNullAssertion.AssertNotNull(levelable, nameof(levelable));
            levelAssertion.AssertBelowMaxLevel(levelable);
            numberAssertion.AssertNonNegative(levelable.ExperiencePerAction);
        }
    }
}