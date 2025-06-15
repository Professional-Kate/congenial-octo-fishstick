using IdelPog.Engine.Orchestration;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Controller
{
    public class JobController(IJobMediator jobMediator) : IJobController
    {
        public ServiceResponse CompleteJob(JobType jobType)
        {
            ServiceResponse response = jobMediator.ProcessJobAction(jobType);
            if (response.IsSuccess == false)
            {
                // TODO : Log to file
                Console.WriteLine(response.Message);
            }
            
            return response;
        }
    }
}