using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Structures.Level
{
    public interface ILevelProgressFactory
    {
        public LevelProgress CreateLevelProgress(Levelable levelable);
    }
}