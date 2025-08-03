using IdelPog.Common.Level.Pipelines;
using IdelPog.Common.Structures;

namespace IdelPog.Common.Level.Experience
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