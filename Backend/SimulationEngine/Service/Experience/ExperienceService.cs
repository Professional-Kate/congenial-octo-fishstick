using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Service
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