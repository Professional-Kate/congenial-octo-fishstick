using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Runtime.Systems;
using Console.Types;

namespace Console.Commands.Domains
{
    public class PermissionDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.PERMISSION;
        public CommandDocumentation CommandDocumentation => new() { Syntax = "permission <ActionType> <CommandDomain>", Description = "Add or Remove permission for a domain"};
        
        private readonly IArgumentResolverPipeline<PermissionUpdateArguments> _permissionUpdatePipeline;
        private readonly IAssertArgumentLength _assertArgumentLength;
        private readonly IPermissionService _permissionService;
        
        public PermissionDomainResolver(IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline, IPermissionService permissionService, IAssertArgumentLength assertArgumentLength)
        {
            _permissionUpdatePipeline = permissionUpdatePipeline;
            _permissionService = permissionService;
            _assertArgumentLength = assertArgumentLength;
        }
        
        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _assertArgumentLength.Handle(arguments.Length, 2);
            
            PermissionUpdateArguments permissionUpdateArguments = _permissionUpdatePipeline.Resolve(arguments);
            _permissionService.PermissionUpdate(permissionUpdateArguments);
        }
    }
}