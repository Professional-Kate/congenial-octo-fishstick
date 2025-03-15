using System;
using IdelPog.Constants;
using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Validation.Pipelines.Interfaces
{
    /// <seealso cref="AssertLevelable"/>
    public interface ILevelableAsserter
    {
        /// <summary>
        /// Asserts that the passed <see cref="ILevelable"/> is completely valid
        /// </summary>
        /// <param name="levelable">The <see cref="ILevelable"/> you want to verify</param>
        /// <exception cref="ArgumentNullException">If the passed <see cref="ILevelable"/> is null</exception>
        /// <exception cref="MaxLevelException">If the passed <see cref="ILevelable"/> level is <see cref="JobConstants.MAX_JOB_LEVEL"/></exception>
        /// <exception cref="NegativeNumberException">If the experience per action is negative</exception>
        public void AssertLevelable(ILevelable levelable);
    }
}