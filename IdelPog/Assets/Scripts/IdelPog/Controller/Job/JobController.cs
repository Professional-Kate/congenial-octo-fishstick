using System;
using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Controller
{
    public class JobController : Singleton<JobController>, IJobController
    {
        protected IJobMediator Mediator = JobMediator.CreateDefault();
        
        public void CompleteJob(JobType job)
        {
            throw new NotImplementedException();
        }
    }
}