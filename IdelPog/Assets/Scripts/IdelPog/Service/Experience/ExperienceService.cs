using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        public void AddExperience(ILevelable levelable)
        {
            if (levelable == null)
            {
                throw new ArgumentNullException(nameof(levelable));
            }
            
            if (levelable.Level == JobConstants.MAX_JOB_LEVEL)
            {
                throw new MaxLevelException($"Error! Passed Job {levelable} is at max level. Adding experience is not possible!");
            }
            
            if (levelable.ExperiencePerAction <= 0)
            {
                throw new ArgumentException($"Error! Passed Experience amount : {levelable.ExperiencePerAction} is expected to be a positive number.");
            }
            
            int experienceToAdd = levelable.ExperiencePerAction + levelable.Experience;
            levelable.SetExperience(experienceToAdd);
        }
    }
}