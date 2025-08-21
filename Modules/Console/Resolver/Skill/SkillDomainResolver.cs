using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Skill
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.SKILL;

        private readonly IArgumentResolverPipeline<SetSkillArguments> _setSkillArguments;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public SkillDomainResolver(IArgumentResolverPipeline<SetSkillArguments> setSkillArguments, IArgumentCountAssertion argumentCountAssertion)
        {
            _setSkillArguments = setSkillArguments;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);
            
            _setSkillArguments.Resolve(arguments);
        }
    }
}