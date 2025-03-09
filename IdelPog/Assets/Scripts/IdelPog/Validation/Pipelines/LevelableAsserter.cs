using IdelPog.Model;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Pipelines.Interfaces;

namespace IdelPog.Validation.Assertions
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
        
        public void AssertLevelable(Job job)
        {
            _assertNotNull.AssertObjectNotNull(job);
            _assertUnderMaxLevel.AssertLevelIsUnderMax(job);
            _assertPositive.AssertNumberIsPositive(job.ExperiencePerAction);
        }
    }
}