using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Structures
{
    public interface ILevelableUpdateFactory
    {
        public LevelableUpdateDTO CreateLevelableUpdate(Levelable levelable);
    }
}