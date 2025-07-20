using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Types;

namespace Console.Commands.Domains
{
    public class PermissionDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledPermission => CommandDomain.PERMISSION;
        public CommandDocumentation CommandDocumentation => new() { Syntax = "permission <ActionType> <CommandDomain>", Description = "Add or Remove permission for a domain"};
        
        private readonly IArgumentResolverPipeline<PermissionUpdateArguments> _permissionUpdatePipeline;
        private readonly IAssertArgumentLength _assertArgumentLength;
        
        public PermissionDomainResolver(IArgumentResolverPipeline<PermissionUpdateArguments> permissionUpdatePipeline, IAssertArgumentLength assertArgumentLength)
        {
            _permissionUpdatePipeline = permissionUpdatePipeline;
            _assertArgumentLength = assertArgumentLength;
        }
        
        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _assertArgumentLength.Handle(arguments.Length, 2);
            
            PermissionUpdateArguments permissionUpdateArguments = _permissionUpdatePipeline.Resolve(arguments);
            
            // TODO: new System level class
            // ADD: assert not found, factory create domain component, add into ECS
            // REMOVE: assert found, remove from ECS
        }
    }
}