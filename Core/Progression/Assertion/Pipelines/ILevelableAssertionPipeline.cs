using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Progression.Assertion.Pipelines
{
    public interface ILevelableAssertionPipeline
    {
        /// <summary>
        /// Asserts that the passed <see cref="Levelable"/> is completely valid
        /// </summary>
        /// <param name="levelable">The <see cref="Levelable"/> you want to verify</param>
        /// <exception cref="ArgumentNullException">If the passed <see cref="Levelable"/> is null</exception>
        /// <exception cref="MaxLevelException">If the passed <see cref="Levelable"/> level is <see cref="LevelConstants.MAX_LEVEL"/></exception>
        public void AssertLevelable(Levelable levelable);
    }
}