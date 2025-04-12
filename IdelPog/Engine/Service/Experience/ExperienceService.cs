using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Validation.Pipelines.Interfaces;

namespace IdelPog.Engine.Service.Experience
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