using IdelPog.SimulationEngine.Orchestration;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Controller
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