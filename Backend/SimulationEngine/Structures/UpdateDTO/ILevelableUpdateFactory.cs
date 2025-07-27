using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Structures
{
    public interface ILevelableUpdateFactory
    {
        public LevelableUpdateDTO CreateLevelableUpdate(Levelable levelable);
    }
}