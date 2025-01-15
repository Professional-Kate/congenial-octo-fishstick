using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Model;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        public bool AddExperience(Job job, int experience)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }
            
            if (job.Level == JobConstants.MAX_JOB_LEVEL)
            {
                throw new MaxLevelException($"Error! Passed Job {job} is at max level. Cannot add Experience.");
            }
            
            if (experience <= 0)
            {
                throw new ArgumentException($"Error! Passed Experience amount : {experience} is expected to be a positive number.");
            }
            
            job.AddExperience(experience);

            bool canJobLevel = experience >= job.ExperienceToNextLevel;
            return canJobLevel;
        }
    }
}