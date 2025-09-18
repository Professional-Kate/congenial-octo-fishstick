using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Factory
{
    public class LevelProgressFactory : ILevelProgressFactory
    {
        public ReadOnlyLevelable CreateLevelProgress(Levelable levelable)
        {
            return new ReadOnlyLevelable
            {
                Experience = levelable.Experience,
                ExperiencePerAction = levelable.ExperiencePerAction,
                Level = levelable.Level,
                NextLevelExperience = levelable.NextLevelExperience
            };
        }
    }
}