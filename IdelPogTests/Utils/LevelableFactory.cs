using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Utilities.Builders;

namespace IdelPogTests.Utils
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