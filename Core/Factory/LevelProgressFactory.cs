using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Factory
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