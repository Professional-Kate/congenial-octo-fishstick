using System;
using IdelPog.Model;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        private readonly IAssertUnderMaxLevel _assertUnderMaxLevel;

        public ExperienceService(IAssertUnderMaxLevel assertUnderMaxLevel)
        {
            _assertUnderMaxLevel = assertUnderMaxLevel;
        }
        
        public void AddExperience(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }
            
            _assertUnderMaxLevel.AssertLevelIsUnderMax(job);
            
            if (job.ExperiencePerAction <= 0)
            {
                throw new ArgumentException($"Error! Passed Experience amount : {job.ExperiencePerAction} is expected to be a positive number.");
            }
            
            int experienceToAdd = job.ExperiencePerAction;
            job.AddExperience(experienceToAdd);
        }
    }
}