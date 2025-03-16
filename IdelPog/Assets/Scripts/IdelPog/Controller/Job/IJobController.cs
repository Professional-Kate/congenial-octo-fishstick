using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Controller
{
    /// <seealso cref="CompleteJob"/>
    public interface IJobController
    {
        /// <summary>
        /// Calls to complete a job action by using the passed <see cref="JobType"/>
        /// </summary>
        /// <param name="jobType">The <see cref="JobType"/> you want to process a job completion on</param>
        /// <returns>A <see cref="ServiceResponse"/> object on the state of the operation</returns>
        public ServiceResponse CompleteJob(JobType jobType);
    }
}