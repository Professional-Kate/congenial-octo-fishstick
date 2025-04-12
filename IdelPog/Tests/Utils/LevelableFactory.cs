using IdelPog.Main.Structures.Models.Builders.Levelable;
using IdelPog.Main.Structures.Models.Levelable;

namespace IdelPog.Tests.Utils
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
                .Build();
        }
    }
}