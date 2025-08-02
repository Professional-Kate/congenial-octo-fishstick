using IdelPog.Common.Structures;

namespace IdelPog.Common.Factories
{
    public class LevelProgressFactory : ILevelProgressFactory
    {
        public LevelProgress CreateLevelProgress(Levelable levelable)
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