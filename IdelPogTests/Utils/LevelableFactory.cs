using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Utilities.Builders;

namespace IdelPogTests.Utils
{
    internal static class LevelableFactory
    {
        internal static ILevelable CreateLevelable()
        {
            return new Levelable(0, 0, 0, 0);
        }
    }
}