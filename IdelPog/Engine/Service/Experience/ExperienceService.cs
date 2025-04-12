using IdelPog.Engine.Structures;
using IdelPog.Engine.Validation.Pipelines;

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