using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Structures.Level
{
    public class LevelProgressFactory : ILevelProgressFactory
    {
        public LevelProgress CreateLevelableUpdate(Levelable levelable)
        {
            return new LevelProgress
            {
                Experience = levelable.Experience,
                ExperiencePerAction = levelable.ExperiencePerAction,
                Level = levelable.Level,
                NextLevelExperience = levelable.NextLevelExperience
            };
        }
    }
}