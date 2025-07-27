using IdelPog.Common.Structures;

namespace IdelPogTests.Utils
{
    internal static class LevelableFactory
    {
        internal static Levelable CreateLevelable()
        {
            return new Levelable(0, 0, 0, 0);
        }
    }
}