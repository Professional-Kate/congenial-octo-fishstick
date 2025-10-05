namespace IdelPog.Progression.Assertion.Interface
{
    public interface IIDMatchesAssertion<in TID>
    { 
        public void AssertIDMatches(TID actualID, TID expectedID);
    }
}