using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Service
{
    public class ExperienceService(ILevelableAssertionPipeline levelableAssertionPipeline) : IExperienceService
    {
        public void AddExperience(Levelable levelable)
        {
            levelableAssertionPipeline.AssertLevelable(levelable);

            uint newExperience = levelable.ExperiencePerAction + levelable.Experience;
            levelable.Experience = newExperience;
        }
    }
}