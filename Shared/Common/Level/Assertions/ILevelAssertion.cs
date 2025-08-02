using IdelPog.Common.Structures;

namespace IdelPog.Common.Level.Assertions
{
    public interface ILevelAssertion
    {
        public void AssertBelowMaxLevel(Levelable levelable);
    }
}