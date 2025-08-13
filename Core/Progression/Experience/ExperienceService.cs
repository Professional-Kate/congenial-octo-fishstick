using IdelPog.Core.Progression.Assertion.Pipelines;

namespace IdelPog.Core.Progression.Experience
{
    public class ExperienceService : IExperienceService
    {
        private readonly ILevelableAssertionPipeline _levelableAssertionPipeline;

        public ExperienceService(ILevelableAssertionPipeline levelableAssertionPipeline)
        {
            _levelableAssertionPipeline = levelableAssertionPipeline;
        }

        public void AddExperience(Levelable levelable)
        {
            _levelableAssertionPipeline.AssertLevelable(levelable);

            uint newExperience = levelable.ExperiencePerAction + levelable.Experience;
            levelable.Experience = newExperience;
        }
    }
}