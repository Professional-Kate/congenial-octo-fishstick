using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Console.Command.Resolver.Pipeline;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Dispatcher.Single;

namespace IdelPog.Console.Command.Domain
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public Types.Domain HandledDomain => Types.Domain.SKILL;
        public CommandDocumentation CommandDocumentation => new() { Syntax = "skill change <SkillID>", Description = "Change to another skill!!! Exciting times!" };

        private readonly IArgumentResolverPipeline<SetSkillArguments> _argumentResolverPipeline;
        private readonly IDispatchOne<SetSkill> _skillChangeDispatcher;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public SkillDomainResolver(IArgumentResolverPipeline<SetSkillArguments> argumentResolverPipeline, IDispatchOne<SetSkill> skillChangeDispatcher, IArgumentCountAssertion argumentCountAssertion)
        {
            _argumentResolverPipeline = argumentResolverPipeline;
            _skillChangeDispatcher = skillChangeDispatcher;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);
            SetSkillArguments setSkillArguments = _argumentResolverPipeline.Resolve(arguments);

            SetSkill setSkill = new()
            {
                SkillID = setSkillArguments.SkillID
            };
            
            _skillChangeDispatcher.Dispatch(setSkill);
        }
    }
}