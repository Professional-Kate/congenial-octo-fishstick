using IdelPog.Model;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Orchestration
{
    /// <seealso cref="ProcessJobAction"/>
    public interface IJobMediator
    {
        /// <summary>
        /// Processes a <see cref="Job"/> action. What each <see cref="Job"/> does per action is defined by the <see cref="Job"/> itself
        /// </summary>
        /// <param name="jobType">The <see cref="JobType"/> you want to process an action on</param>
        /// <returns>A <see cref="ServiceResponse"/> which will tell you if the operation was successful</returns>
        /// <remarks>
        /// This method will only ever return a <see cref="ServiceResponse"/>, so, if anything goes wrong it'll be wrapped in this object.
        /// </remarks>
        public ServiceResponse ProcessJobAction(JobType jobType);
    }
}