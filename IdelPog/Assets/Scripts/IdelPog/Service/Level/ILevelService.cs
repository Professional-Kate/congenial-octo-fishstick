using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures.Models.Levelable;
using log4net.Core;

namespace IdelPog.Service.Level
{
    /// <seealso cref="CanJobLevel"/>
    /// <seealso cref="LevelUpJob"/>
    public interface ILevelService
    {
        /// <summary>
        /// Calculate and return if the passed <see cref="Job"/> can level up
        /// </summary>
        /// <param name="levelable">The <see cref="Job"/> you want to check if it can level up</param>
        /// <returns>If the <see cref="Job"/> can level up</returns>
        public bool CanJobLevel(ILevelable levelable) => levelable.Experience >= levelable.NextLevelExperience;
    
        /// <summary>
        /// Invoke this to level up the passed <see cref="Job"/>. Will increase the <see cref="Job"/>.<see cref="Level"/>
        /// </summary>
        /// <param name="levelable">The <see cref="Job"/> you want to level</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="Job"/> is null</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="Job"/> is at max level</exception>
        /// <remarks>
        /// This method will calculate a new <see cref="ILevelable.NextLevelExperience"/>
        /// </remarks>
        public void LevelUpJob(ILevelable levelable);
    }
}