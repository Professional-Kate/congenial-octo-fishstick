using IdelPog.Structures.Enums;

namespace IdelPog.Controller
{
    public interface IJobController
    {
        public void CompleteJob(JobType job);
    }
}