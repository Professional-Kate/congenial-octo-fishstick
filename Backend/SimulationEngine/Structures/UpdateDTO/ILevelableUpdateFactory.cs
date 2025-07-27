namespace IdelPog.SimulationEngine.Models
{
    public interface ILevelableUpdateFactory
    {
        public LevelableUpdateDTO CreateLevelableUpdate(Levelable levelable);
    }
}