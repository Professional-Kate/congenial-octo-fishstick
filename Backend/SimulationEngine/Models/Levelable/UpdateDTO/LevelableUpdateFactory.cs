namespace IdelPog.SimulationEngine.Models
{
    public class LevelableUpdateFactory : ILevelableUpdateFactory
    {
        public LevelableUpdateDTO CreateLevelableUpdate(ILevelable levelable)
        {
            return new LevelableUpdateDTO
            {
                Experience = levelable.Experience,
                ExperiencePerAction = levelable.ExperiencePerAction,
                Level = levelable.Level,
                NextLevelExperience = levelable.NextLevelExperience,
            };
        }
    }
}