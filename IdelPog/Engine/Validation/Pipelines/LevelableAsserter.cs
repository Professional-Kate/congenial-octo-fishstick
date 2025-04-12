using IdelPog.Engine.Structures;
using IdelPog.Engine.Validation.Assertions;

namespace IdelPog.Engine.Validation.Pipelines
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