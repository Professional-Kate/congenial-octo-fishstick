using Console.Assertions;
using Console.Commands.Domains;
using Console.Runtime.Systems;
using Console.Types;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;

namespace Console.Commands
{
    public class CommandResolverMediator : ICommandResolverMediator
    {
        private readonly IAssetRepository<Domain, ICommandDomainResolver> _commandResolverMap;
        private readonly IDomainPermissionChecker  _domainPermissionChecker;
        private readonly IAssertFound _assertFound;
        private readonly IAssertSpanNotEmpty _assertSpanNotEmpty;
        private readonly IAssertHasPermission _assertHasPermission;

        public CommandResolverMediator(IAssetRepository<Domain, ICommandDomainResolver> commandResolverMap, IDomainPermissionChecker domainPermissionChecker, IAssertFound assertFound,  IAssertSpanNotEmpty assertSpanNotEmpty, IAssertHasPermission assertHasPermission)
        {
            _commandResolverMap = commandResolverMap;
            _domainPermissionChecker = domainPermissionChecker;
            _assertFound = assertFound;
            _assertSpanNotEmpty = assertSpanNotEmpty;
            _assertHasPermission = assertHasPermission;
        }
        
        public void ResolveCommand(Domain domain, ReadOnlySpan<string> arguments)
        {
            _assertSpanNotEmpty.Handle(arguments);
            _assertFound.AssertItemIsFound(domain, () => _commandResolverMap.Contains(domain));
            _assertHasPermission.Handle(_domainPermissionChecker.IsAllowed(domain), domain);
            
            ICommandDomainResolver commandResolver = _commandResolverMap.Get(domain);
            
            commandResolver.Resolve(arguments);
        }
    }
}