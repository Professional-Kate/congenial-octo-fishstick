namespace IdelPog.SimulationEngine.Models
{
    public interface ILevelableUpdateFactory
    {
        public LevelableUpdateDTO CreateLevelableUpdate(ILevelable levelable);
    }
}