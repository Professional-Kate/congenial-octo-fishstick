using Console.Commands.Domains.Arguments;
using Console.Types;
using IdelPog.Common.Enums;

namespace Console.Commands.Resolver.Pipelines
{
    public class PermissionUpdateResolver : IArgumentResolverPipeline<PermissionUpdateArguments>
    {
        private readonly IArgumentResolver<ActionType> _actionTypeResolver;
        private readonly IArgumentResolver<Domain> _commandDomainResolver;

        public PermissionUpdateResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<Domain> commandDomainResolver)
        {
            _actionTypeResolver = actionTypeResolver;
            _commandDomainResolver = commandDomainResolver;
        }

        public PermissionUpdateArguments Resolve(ReadOnlySpan<string> arguments)
        {
            ActionType actionType = _actionTypeResolver.Resolve(arguments[0]);
            Domain domain = _commandDomainResolver.Resolve(arguments[1]);

            return new PermissionUpdateArguments { ActionType = actionType, Domain = domain };
        }
    }
}