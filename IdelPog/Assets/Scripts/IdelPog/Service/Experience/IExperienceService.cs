using System;
using IdelPog.Exceptions;
using IdelPog.Model;

namespace IdelPog.Service
{
    /// <seealso cref="AddExperience"/>
    public interface IExperienceService
    {
        /// <summary>
        /// Calculate and return if the passed <see cref="Job"/> can level up
        /// </summary>
        /// <param name="job">The <see cref="Job"/> you want to check if it can level up</param>
        /// <returns>If the <see cref="Job"/> can level up</returns>
        public bool CanJobLevel(Job job) => job.Experience >= job.NextLevelExperience;
        
        /// <summary>
        /// Adds Experience to the passed <see cref="Job"/>
        /// </summary>
        /// <param name="job">The <see cref="Job"/> you want to add experience to</param>
        /// <exception cref="ArgumentException">Will be thrown if the <see cref="Job"/>s <see cref="Job.ExperiencePerAction"/> is below or equal to zero</exception>
        /// <exception cref="MaxLevelException">Will be thrown if the passed <see cref="Job"/> is at max level</exception>
        /// <exception cref="ArgumentNullException">Will be thrown if the passed <see cref="Job"/> is null</exception>
        /// <remarks>
        /// The passed <see cref="Job"/> is expected to be a reference. The new state of the <see cref="Job"/> won't be returned
        /// </remarks>
        public void AddExperience(Job job); 
    }
}