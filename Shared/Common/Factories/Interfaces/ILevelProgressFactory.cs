using IdelPog.Common.Structures;

namespace IdelPog.Common.Factories
{
    public interface ILevelProgressFactory
    {
        public LevelProgress CreateLevelProgress(Levelable levelable);
    }
}