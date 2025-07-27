using Console.Assertions;
using Console.Commands.Domains.Arguments;
using Console.Commands.Resolver.Pipelines;
using Console.Runtime.Systems;
using Console.Types;

namespace Console.Commands.Domains
{
    public class PermissionDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.PERMISSION;
        public CommandDocumentation CommandDocumentation => new()
            { Syntax = "permission <ActionType> <CommandDomain>", Description = "Add or Remove permission for a domain" };

        private readonly IArgumentResolverPipeline<PermissionUpdateArguments> _permissionUpdatePipeline;
        private readonly IArgumentCountAssertion _argumentCountAssertion;
        private readonly IPermissionService _permissionService;

        public PermissionDomainResolver(IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline, IPermissionService permissionService,
            IArgumentCountAssertion argumentCountAssertion)
        {
            _permissionUpdatePipeline = permissionUpdatePipeline;
            _permissionService = permissionService;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 2);

            PermissionUpdateArguments permissionUpdateArguments = _permissionUpdatePipeline.Resolve(arguments);
            _permissionService.PermissionUpdate(permissionUpdateArguments);
        }
    }
}