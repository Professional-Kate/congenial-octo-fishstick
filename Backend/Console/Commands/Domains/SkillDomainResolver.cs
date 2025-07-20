using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch;

namespace Console.Commands.Domains
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.SKILL;
        public CommandDocumentation CommandDocumentation { get; } = new() { Syntax = "skill change <SkillID>", Description = "Change to another skill!!! Exciting times!"};

        private readonly IArgumentResolverPipeline<SkillChangeArguments> _argumentResolverPipeline;
        private readonly IDispatchOne<SkillChange> _skillChangeDispatcher;
        private readonly ISkillChangeFactory _skillChangeFactory;
        private readonly IAssertArgumentLength _assertArgumentLength;

        public SkillDomainResolver(IArgumentResolverPipeline<SkillChangeArguments> argumentResolverPipeline, IDispatchOne<SkillChange> skillChangeDispatcher, ISkillChangeFactory skillChangeFactory, IAssertArgumentLength assertArgumentLength)
        {
            _argumentResolverPipeline = argumentResolverPipeline;
            _skillChangeDispatcher = skillChangeDispatcher;
            _skillChangeFactory = skillChangeFactory;
            _assertArgumentLength = assertArgumentLength;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _assertArgumentLength.Handle(arguments.Length, 2);
            SkillChangeArguments skillChangeArguments = _argumentResolverPipeline.Resolve(arguments);
            
            _skillChangeDispatcher.Dispatch(_skillChangeFactory.CreateSkillChange(skillChangeArguments.SkillID));
        }
    }
}