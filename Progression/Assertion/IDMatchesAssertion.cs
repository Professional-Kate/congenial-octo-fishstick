using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Assertion
{
    public sealed class IDMatchesAssertion<TID> : BaseAssertion, IIDMatchesAssertion<TID> where TID : Enum
    {
        public IDMatchesAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertIDMatches(TID actualID, TID expectedID)
        {
            Assert<IDMismatchException<TID>>(() =>
            {
                if (actualID.Equals(expectedID) == false)
                {
                    throw new IDMismatchException<TID>(actualID, expectedID);
                }
            });
        }
    }
}