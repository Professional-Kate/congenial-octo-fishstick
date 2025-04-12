using IdelPogTemp.Main.Structures.Models.Levelable;
using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Service.Experience
{
    /// <seealso cref="AddExperience"/>
    public interface IExperienceService
    {
        /// <summary>
        /// Adds Experience to the passed <see cref="ILevelable"/>
        /// </summary>
        /// <param name="levelable">The <see cref="ILevelable"/> you want to add experience to</param>
        /// <exception cref="ArgumentException">Will be thrown if the <see cref="ILevelable"/>s <see cref="ILevelable.ExperiencePerAction"/> is below or equal to zero</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="ILevelable"/> is at max level</exception>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="ILevelable"/> is null</exception>
        /// <remarks>
        /// The passed <see cref="ILevelable"/> is expected to be a reference. The new state of the <see cref="ILevelable"/> won't be returned
        /// </remarks>
        public void AddExperience(ILevelable levelable); 
    }
}