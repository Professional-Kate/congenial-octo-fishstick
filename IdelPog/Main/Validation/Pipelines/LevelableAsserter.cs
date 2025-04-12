using IdelPogTemp.Main.Structures.Models.Levelable;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Pipelines.Interfaces;

namespace IdelPogTemp.Main.Validation.Pipelines
{
    /// <inheritdoc cref="ILevelableAsserter"/>
    public class LevelableAsserter : ILevelableAsserter
    {
        private readonly IAssertUnderMaxLevel _assertUnderMaxLevel;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertPositive _assertPositive;
        
        public LevelableAsserter(IAssertUnderMaxLevel assertUnderMaxLevel, IAssertNotNull assertNotNull, IAssertPositive assertPositive)
        {
            _assertUnderMaxLevel = assertUnderMaxLevel;
            _assertNotNull = assertNotNull;
            _assertPositive = assertPositive;
        }
        
        public void AssertLevelable(ILevelable levelable)
        {
            _assertNotNull.AssertObjectNotNull(levelable);
            _assertUnderMaxLevel.AssertLevelIsUnderMax(levelable);
            _assertPositive.AssertNumberIsPositive(levelable.ExperiencePerAction);
        }
    }
}