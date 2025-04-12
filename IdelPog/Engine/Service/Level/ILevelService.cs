using IdelPog.Engine.Structures;
using IdelPog.Engine.Validation.Exceptions;

namespace IdelPog.Engine.Service
{
    /// <seealso cref="CanJobLevel"/>
    /// <seealso cref="LevelUpJob"/>
    public interface ILevelService
    {
    
        /// <summary>
        /// Calculate and return if the passed <see cref="ILevelable"/> can level up
        /// </summary>
        /// <param name="levelable">The <see cref="ILevelable"/> you want to check if it can level up</param>
        /// <returns>If the <see cref="ILevelable"/> can level up</returns>
        public bool CanJobLevel(ILevelable levelable) => levelable.Experience >= levelable.NextLevelExperience;
    
        /// <summary>
        /// Invoke this to level up the passed <see cref="ILevelable"/>
        /// </summary>
        /// <param name="levelable">The <see cref="ILevelable"/> you want to level</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="ILevelable"/> is null</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="ILevelable"/> is at max level</exception>
        /// <remarks>
        /// This method will calculate a new <see cref="ILevelable.NextLevelExperience"/>
        /// </remarks>
        public void LevelUpJob(ILevelable levelable);
    }
}