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
        private readonly IDomainPermissionChecker _domainPermissionChecker;
        private readonly IAssertFound _assertFound;
        private readonly ISpanAssertion _spanAssertion;
        private readonly IDomainPermissionAssertion _domainPermissionAssertion;

        public CommandResolverMediator(IAssetRepository<Domain, ICommandDomainResolver> commandResolverMap, IDomainPermissionChecker domainPermissionChecker,
            IAssertFound assertFound, ISpanAssertion spanAssertion, IDomainPermissionAssertion domainPermissionAssertion)
        {
            _commandResolverMap = commandResolverMap;
            _domainPermissionChecker = domainPermissionChecker;
            _assertFound = assertFound;
            _spanAssertion = spanAssertion;
            _domainPermissionAssertion = domainPermissionAssertion;
        }

        public void ResolveCommand(Domain domain, ReadOnlySpan<string> arguments)
        {
            _spanAssertion.AssertNotEmpty(arguments);
            _assertFound.AssertItemIsFound(domain, () => _commandResolverMap.Contains(domain));
            _domainPermissionAssertion.AssertHasPermission(_domainPermissionChecker.IsAllowed(domain), domain);

            ICommandDomainResolver commandResolver = _commandResolverMap.Get(domain);

            commandResolver.Resolve(arguments);
        }
    }
}