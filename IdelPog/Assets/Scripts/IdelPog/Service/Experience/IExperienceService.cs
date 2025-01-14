using IdelPog.Model;
using IdelPog.Structures;

namespace IdelPog.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AddExperience"/>
    public interface IExperienceService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="experience"></param>
        /// <returns></returns>
        public ServiceResponse AddExperience(Job job, uint experience); 
    }
}