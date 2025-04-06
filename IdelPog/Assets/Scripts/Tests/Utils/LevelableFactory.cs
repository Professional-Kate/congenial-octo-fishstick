using IdelPog.Structures.Builders;
using IdelPog.Structures.Models;
using IdelPog.Structures.Models.Levelable;

namespace Tests.Utils
{
    internal static class LevelableFactory
    {
        internal static ILevelable CreateLevelable()
        {
            return LevelableBuilder.Builder()
                .Level(0)
                .NextLevelExperience(0)
                .ExperiencePerAction(0)
                .Experience(0)
                .LevelRewards(new LevelRewards())
                .Build();
        }
    }
}