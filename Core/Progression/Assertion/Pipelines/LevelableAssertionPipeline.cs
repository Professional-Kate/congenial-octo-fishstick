using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Progression.Assertion.Pipelines
{
    /// <inheritdoc cref="ILevelableAssertionPipeline"/>
    public class LevelableAssertionPipeline : ILevelableAssertionPipeline
    {
        private readonly ILevelAssertion _levelAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public LevelableAssertionPipeline(ILevelAssertion levelAssertion, IObjectNullAssertion objectNullAssertion)
        {
            _levelAssertion = levelAssertion;
            _objectNullAssertion = objectNullAssertion;
        }

        public void AssertLevelable(Levelable levelable)
        {
            _objectNullAssertion.AssertNotNull(levelable, nameof(levelable));
            _levelAssertion.AssertBelowMaxLevel(levelable);
        }
    }
}