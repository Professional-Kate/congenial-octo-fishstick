using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Skill
{
    public class SkillDomainResolver : IDomainResolver
    {
        public Domain HandledDomain => Domain.SKILL;

        private readonly ISubDomainResolver _setSkillSubDomains;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public SkillDomainResolver(ISubDomainResolver setSkillSubDomains, IArgumentCountAssertion argumentCountAssertion)
        {
            _setSkillSubDomains = setSkillSubDomains;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);
            
            _setSkillSubDomains.Resolve(arguments);
        }
    }
}