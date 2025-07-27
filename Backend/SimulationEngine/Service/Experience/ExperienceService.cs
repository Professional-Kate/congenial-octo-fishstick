using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Service
{
    public class ExperienceService(ILevelableAssertionPipeline levelableAssertionPipeline) : IExperienceService
    {
        public void AddExperience(ILevelable levelable)
        {
            levelableAssertionPipeline.AssertLevelable(levelable);

            int experienceToAdd = levelable.ExperiencePerAction + levelable.Experience;
            levelable.SetExperience(experienceToAdd);
        }
    }
}