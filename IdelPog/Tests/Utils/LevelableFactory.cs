using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Utilities.Builders.Levelable;

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