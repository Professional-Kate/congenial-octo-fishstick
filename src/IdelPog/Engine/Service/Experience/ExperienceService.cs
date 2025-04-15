using IdelPog.Engine.Assertions.Pipelines;
using IdelPog.Engine.Models;

namespace IdelPog.Engine.Service
{
    public class ExperienceService(ILevelableAsserter levelableAsserter) : IExperienceService
    {
        public void AddExperience(ILevelable levelable)
        {
            levelableAsserter.AssertLevelable(levelable);

            int experienceToAdd = levelable.ExperiencePerAction + levelable.Experience;
            levelable.SetExperience(experienceToAdd);
        }
    }
}