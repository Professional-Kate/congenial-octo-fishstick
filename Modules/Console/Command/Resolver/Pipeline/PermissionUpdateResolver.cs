using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Command.Resolver.Pipeline
{
    public class PermissionUpdateResolver : IArgumentResolverPipeline<PermissionUpdateArguments>
    {
        private readonly IArgumentResolver<ActionType> _actionTypeResolver;
        private readonly IArgumentResolver<Types.Domain> _commandDomainResolver;

        public PermissionUpdateResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<Types.Domain> commandDomainResolver)
        {
            _actionTypeResolver = actionTypeResolver;
            _commandDomainResolver = commandDomainResolver;
        }

        public PermissionUpdateArguments Resolve(ReadOnlySpan<string> arguments)
        {
            ActionType actionType = _actionTypeResolver.Resolve(arguments[0]);
            Types.Domain domain = _commandDomainResolver.Resolve(arguments[1]);

            return new PermissionUpdateArguments { ActionType = actionType, Domain = domain };
        }
    }
}