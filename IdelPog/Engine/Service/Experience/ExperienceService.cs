using IdelPog.Engine.Structures.Models.Levelable;
using IdelPog.Engine.Validation.Pipelines.Interfaces;

namespace IdelPog.Engine.Service.Experience
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