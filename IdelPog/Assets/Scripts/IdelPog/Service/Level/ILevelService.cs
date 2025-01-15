using System;
using IdelPog.Model;

namespace IdelPog.Service
{
    /// <seealso cref="LevelUpJob"/>
    public interface ILevelService
    {
        /// <summary>
        /// Invoke this to level up the passed <see cref="Job"/>. Will increase the <see cref="Job"/>.<see cref="Job.Level"/>
        /// </summary>
        /// <param name="job">The <see cref="Job"/> you want to level</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="Job"/> is null</exception>
        /// <remarks>
        /// This method will calculate a new <see cref="Job.NextLevelExperience"/>
        /// </remarks>
        public void LevelUpJob(Job job);
    }
}