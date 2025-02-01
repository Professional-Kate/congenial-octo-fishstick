using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using UnityEngine;

namespace IdelPog.Controller
{
    public class JobController : Singleton<JobController>, IJobController
    {
        protected IJobMediator Mediator = new JobMediator();
        
        public void CompleteJob(JobType jobType)
        {
            ServiceResponse response = Mediator.ProcessJobAction(jobType);
            if (response.IsSuccess == false)
            {
                // TODO : Log to file
                Debug.Log(response.Message);
            }
        }
    }
}