using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Exceptions;

namespace IdelPog.SimulationEngine.Service
{
    /// <seealso cref="AddExperience"/>
    public interface IExperienceService
    {
        /// <summary>
        /// Adds Experience to the passed <see cref="Levelable"/>
        /// </summary>
        /// <param name="levelable">The <see cref="Levelable"/> you want to add experience to</param>
        /// <exception cref="ArgumentException">Will be thrown if the <see cref="Levelable"/>s <see cref="Levelable.ExperiencePerAction"/> is below or equal to zero</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="Levelable"/> is at max level</exception>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="Levelable"/> is null</exception>
        /// <remarks>
        /// The passed <see cref="Levelable"/> is expected to be a reference. The new state of the <see cref="Levelable"/> won't be returned
        /// </remarks>
        public void AddExperience(Levelable levelable);
    }
}