using System;
using IdelPog.Exceptions;
using IdelPog.Model;

namespace IdelPog.Service
{
    /// <seealso cref="AddExperience"/>
    public interface IExperienceService
    {
        /// <summary>
        /// Adds Experience to the passed <see cref="Job"/>
        /// </summary>
        /// <param name="job">The <see cref="Job"/> you want to add experience to</param>
        /// <param name="experience">The amount of Experience you want to add into the <see cref="Job"/></param>
        /// <returns>A boolean on if the <see cref="Job"/> should level up</returns>
        /// <exception cref="ArgumentException">Will be thrown if the passed experience is below or equal to zero</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="Job"/> is at max level</exception>
        /// <remarks>
        /// The passed <see cref="Job"/> is expected to be a reference. The new state of the <see cref="Job"/> won't be returned
        /// </remarks>
        public bool AddExperience(Job job, int experience); 
    }
}