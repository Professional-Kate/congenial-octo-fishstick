using IdelPogTemp.Main.Structures.Models.Levelable;
using IdelPogTemp.Main.Validation.Pipelines.Interfaces;

namespace IdelPogTemp.Main.Service.Experience
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