using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using UnityEngine;

namespace IdelPog.Controller
{
    public class JobController : IJobController
    {
        private readonly IJobMediator _jobMediator;

        public JobController(IJobMediator jobMediator)
        {
            _jobMediator = jobMediator;
        }
        
        public void CompleteJob(JobType jobType)
        {
            ServiceResponse response = _jobMediator.ProcessJobAction(jobType);
            if (response.IsSuccess == false)
            {
                // TODO : Log to file
                Debug.Log(response.Message);
            }
        }
    }
}