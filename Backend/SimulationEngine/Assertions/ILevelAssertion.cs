using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Assertions
{
    public interface ILevelAssertion
    {
        public void AssertBelowMaxLevel(Levelable levelable);
    }
}