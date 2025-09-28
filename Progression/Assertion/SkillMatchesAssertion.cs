using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Assertion
{
    public sealed class SkillMatchesAssertion<TID> : BaseAssertion, ISkillMatchesAssertion<TID> where TID : Enum
    {
        public SkillMatchesAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertSkillMatches(TID actualID, TID expectedID)
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