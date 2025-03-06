using System;
using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Validation;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        public void AddExperience(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }
            
            if (job.Level == JobConstants.MAX_JOB_LEVEL)
            {
                throw new MaxLevelException(job, GetType());
            }
            
            if (job.ExperiencePerAction <= 0)
            {
                throw new ArgumentException($"Error! Passed Experience amount : {job.ExperiencePerAction} is expected to be a positive number.");
            }
            
            int experienceToAdd = job.ExperiencePerAction;
            job.AddExperience(experienceToAdd);
        }
    }
}