using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Progression.Level
{
    public interface ILevelService
    {
        /// <summary>
        /// Calculate and return if the passed <see cref="Levelable"/> can level up
        /// </summary>
        /// <param name="levelable">The <see cref="Levelable"/> you want to check if it can level up</param>
        /// <returns>If the <see cref="Levelable"/> can level up</returns>
        public bool CanLevel(Levelable levelable)
        {
            return levelable.Experience >= levelable.NextLevelExperience;
        }

        /// <summary>
        /// Invoke this to level up the passed <see cref="Levelable"/>
        /// </summary>
        /// <param name="levelable">The <see cref="Levelable"/> you want to level</param>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="Levelable"/> is null</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="Levelable"/> is at max level</exception>
        /// <remarks>
        /// This method will calculate a new <see cref="Levelable.NextLevelExperience"/>
        /// </remarks>
        public void LevelUp(Levelable levelable);
    }
}