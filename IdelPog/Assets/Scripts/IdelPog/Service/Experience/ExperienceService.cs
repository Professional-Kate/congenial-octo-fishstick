using IdelPog.Model;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        private readonly ILevelableAsserter _levelableAsserter;

        public ExperienceService(ILevelableAsserter levelableAsserter)
        {
            _levelableAsserter = levelableAsserter;
        }
        
        public void AddExperience(Job job)
        {
           _levelableAsserter.AssertLevelable(job);
            
            int experienceToAdd = job.ExperiencePerAction;
            job.AddExperience(experienceToAdd);
        }
    }
}