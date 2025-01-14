using System;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Controller
{
    public class JobController : Singleton<JobController>, IJobController
    {
        public void CompleteJob(JobType job)
        {
            throw new NotImplementedException();
        }
    }
}