using IdelPog.Structures.Models.Levelable;
using IdelPog.Validation.Pipelines.Interfaces;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        private readonly ILevelableAsserter _levelableAsserter;

        public ExperienceService(ILevelableAsserter levelableAsserter)
        {
            _levelableAsserter = levelableAsserter;
        }
        
        public void AddExperience(ILevelable levelable)
        {
            _levelableAsserter.AssertLevelable(levelable);

            int experienceToAdd = levelable.ExperiencePerAction + levelable.Experience;
            levelable.SetExperience(experienceToAdd);
        }
    }
}