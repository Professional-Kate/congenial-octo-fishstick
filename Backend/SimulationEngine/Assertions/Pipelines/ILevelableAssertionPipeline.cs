using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Exceptions;

namespace IdelPog.SimulationEngine.Assertions.Pipelines
{
    /// <seealso cref="AssertLevelable"/>
    public interface ILevelableAssertionPipeline
    {
        /// <summary>
        /// Asserts that the passed <see cref="Levelable"/> is completely valid
        /// </summary>
        /// <param name="levelable">The <see cref="Levelable"/> you want to verify</param>
        /// <exception cref="ArgumentNullException">If the passed <see cref="Levelable"/> is null</exception>
        /// <exception cref="MaxLevelException">If the passed <see cref="Levelable"/> level is <see cref="SkillConstants.MAX_SKILL_LEVEL"/></exception>
        /// <exception cref="NegativeNumberException">If the experience per action is negative</exception>
        public void AssertLevelable(Levelable levelable);
    }
}