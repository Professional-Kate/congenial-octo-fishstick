using IdelPog.Console.Argument.Interface;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Resolver.Permission
{
    public class PermissionUpdateResolver : ISubDomainResolver
    {
        private readonly IArgumentResolver<ActionType> _actionTypeResolver;
        private readonly IArgumentResolver<Domain> _commandDomainResolver;

        public PermissionUpdateResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<Domain> commandDomainResolver)
        {
            _actionTypeResolver = actionTypeResolver;
            _commandDomainResolver = commandDomainResolver;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            ActionType actionType = _actionTypeResolver.Resolve(arguments[0]);
            Domain domain = _commandDomainResolver.Resolve(arguments[1]);

            return new PermissionUpdateArguments { ActionType = actionType, Domain = domain };
        }
    }
}