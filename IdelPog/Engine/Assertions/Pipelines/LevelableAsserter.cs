using IdelPog.Engine.Models;
using IdelPog.Engine.Validation.Assertions;

namespace IdelPog.Engine.Assertions.Pipelines
{
    /// <inheritdoc cref="ILevelableAsserter"/>
    public class LevelableAsserter(IAssertUnderMaxLevel assertUnderMaxLevel, IAssertNotNull assertNotNull, IAssertPositive assertPositive)
        : ILevelableAsserter
    {
        public void AssertLevelable(ILevelable levelable)
        {
            assertNotNull.AssertObjectNotNull(levelable);
            assertUnderMaxLevel.AssertLevelIsUnderMax(levelable);
            assertPositive.AssertNumberIsPositive(levelable.ExperiencePerAction);
        }
    }
}