using IdelPog.Engine.Orchestration.Job;
using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;

namespace IdelPog.Engine.Controller.Job
{
    public class JobController : IJobController
    {
        private readonly IJobMediator _jobMediator;

        public JobController(IJobMediator jobMediator)
        {
            _jobMediator = jobMediator;
        }
        
        public ServiceResponse CompleteJob(JobType jobType)
        {
            ServiceResponse response = _jobMediator.ProcessJobAction(jobType);
            if (response.IsSuccess == false)
            {
                // TODO : Log to file
                Console.WriteLine(response.Message);
            }
            
            return response;
        }
    }
}