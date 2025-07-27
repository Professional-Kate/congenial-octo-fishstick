using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Assertions
{
    public interface ILevelAssertion
    {
        public void AssertBelowMaxLevel(Levelable levelable);
    }
}