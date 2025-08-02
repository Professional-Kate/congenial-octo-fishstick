using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Structures.Level
{
    public interface ILevelProgressFactory
    {
        public LevelProgress CreateLevelableUpdate(Levelable levelable);
    }
}