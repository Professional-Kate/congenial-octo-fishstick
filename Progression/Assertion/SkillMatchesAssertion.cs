using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Assertion
{
    public sealed class SkillMatchesAssertion : BaseAssertion, ISkillMatchesAssertion
    {
        public SkillMatchesAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertSkillMatches(SkillID actual, SkillID expected)
        {
            Assert<SkillMismatchException>(() =>
            {
                if (actual != expected)
                {
                    throw new SkillMismatchException(actual, expected);
                }
            });
        }
    }
}