namespace IdelPog.Core.Progression.Assertion
{
    public interface ILevelAssertion
    {
        public void AssertNotAboveMaxLevel(Levelable levelable);
        
        public void AssertBelowMaxLevel(Levelable levelable);
    }
}