using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Assertion
{
    public sealed class IDMatchesAssertion<TID> : IIDMatchesAssertion<TID> where TID : Enum
    {
        public void AssertIDMatches(TID actualID, TID expectedID)
        {
            if (actualID.Equals(expectedID) == false)
            {
                throw new IDMismatchException<TID>(actualID, expectedID);
            }
        }
    }
}