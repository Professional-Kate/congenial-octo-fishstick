using IdelPogTemp.Main.Orchestration.Job;
using IdelPogTemp.Main.Structures;
using IdelPogTemp.Main.Structures.Enums;

namespace IdelPogTemp.Main.Controller.Job
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