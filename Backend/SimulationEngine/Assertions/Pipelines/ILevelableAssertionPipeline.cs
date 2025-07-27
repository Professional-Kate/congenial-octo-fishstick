using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Assertions.Pipelines
{
    /// <seealso cref="AssertLevelable"/>
    public interface ILevelableAssertionPipeline
    {
        /// <summary>
        /// Asserts that the passed <see cref="ILevelable"/> is completely valid
        /// </summary>
        /// <param name="levelable">The <see cref="ILevelable"/> you want to verify</param>
        /// <exception cref="ArgumentNullException">If the passed <see cref="ILevelable"/> is null</exception>
        /// <exception cref="MaxLevelException">If the passed <see cref="ILevelable"/> level is <see cref="SkillConstants.MAX_SKILL_LEVEL"/></exception>
        /// <exception cref="NegativeNumberException">If the experience per action is negative</exception>
        public void AssertLevelable(ILevelable levelable);
    }
}