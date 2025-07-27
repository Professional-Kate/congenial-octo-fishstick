namespace IdelPog.SimulationEngine.Models
{
    public class LevelableUpdateFactory : ILevelableUpdateFactory
    {
        public LevelableUpdateDTO CreateLevelableUpdate(Levelable levelable)
        {
            return new LevelableUpdateDTO
            {
                Experience = levelable.Experience,
                ExperiencePerAction = levelable.ExperiencePerAction,
                Level = levelable.Level,
                NextLevelExperience = levelable.NextLevelExperience
            };
        }
    }
}