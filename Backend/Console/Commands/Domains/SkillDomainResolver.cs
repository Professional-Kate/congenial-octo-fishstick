using Console.Assertions;
using Console.Commands.Domains.Arguments;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch.Single;

namespace Console.Commands.Domains
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.SKILL;
        public CommandDocumentation CommandDocumentation => new() { Syntax = "skill change <SkillID>", Description = "Change to another skill!!! Exciting times!" };

        private readonly IArgumentResolverPipeline<SetSkillArguments> _argumentResolverPipeline;
        private readonly IDispatchOne<SetSkill> _skillChangeDispatcher;
        private readonly ISetSkillFactory _setSkillFactory;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public SkillDomainResolver(IArgumentResolverPipeline<SetSkillArguments> argumentResolverPipeline, IDispatchOne<SetSkill> skillChangeDispatcher,
            ISetSkillFactory setSkillFactory, IArgumentCountAssertion argumentCountAssertion)
        {
            _argumentResolverPipeline = argumentResolverPipeline;
            _skillChangeDispatcher = skillChangeDispatcher;
            _setSkillFactory = setSkillFactory;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);
            SetSkillArguments setSkillArguments = _argumentResolverPipeline.Resolve(arguments);

            _skillChangeDispatcher.Dispatch(_setSkillFactory.Create(setSkillArguments.SkillID));
        }
    }
}