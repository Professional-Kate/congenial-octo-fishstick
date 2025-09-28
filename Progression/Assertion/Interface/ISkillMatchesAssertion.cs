namespace IdelPog.Progression.Assertion.Interface
{
    public interface ISkillMatchesAssertion<in TID>
    { 
        public void AssertSkillMatches(TID actualID, TID expectedID);
    }
}