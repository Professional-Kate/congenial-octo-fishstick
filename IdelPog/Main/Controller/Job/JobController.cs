using IdelPog.Main.Orchestration.Job;
using IdelPog.Main.Structures;
using IdelPog.Main.Structures.Enums;

namespace IdelPog.Main.Controller.Job
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