using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Assertions.Pipelines
{
    /// <inheritdoc cref="ILevelableAssertionPipeline"/>
    public class LevelableAssertionPipeline : ILevelableAssertionPipeline
    {
        private readonly ILevelAssertion _levelAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly INumberAssertion _numberAssertion;

        public LevelableAssertionPipeline(ILevelAssertion levelAssertion, IObjectNullAssertion objectNullAssertion, INumberAssertion numberAssertion)
        {
            _levelAssertion = levelAssertion;
            _objectNullAssertion = objectNullAssertion;
            _numberAssertion = numberAssertion;
        }

        public void AssertLevelable(Levelable levelable)
        {
            _objectNullAssertion.AssertNotNull(levelable, nameof(levelable));
            _levelAssertion.AssertBelowMaxLevel(levelable);
            _numberAssertion.AssertNonNegative(levelable.ExperiencePerAction);
        }
    }
}