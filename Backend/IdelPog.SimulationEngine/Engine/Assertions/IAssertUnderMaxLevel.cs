using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Assertions
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}