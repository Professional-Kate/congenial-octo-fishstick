using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Progression.Assertion.Interface
{
    public interface ISkillMatchesAssertion
    { 
        public void AssertSkillMatches(SkillID actual, SkillID expected);
    }
}