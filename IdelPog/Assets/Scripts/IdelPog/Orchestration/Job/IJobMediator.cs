using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Orchestration
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ProcessJobAction"/>
    public interface IJobMediator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public ServiceResponse ProcessJobAction(JobType job);
    }
}