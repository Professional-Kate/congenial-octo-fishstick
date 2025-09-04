using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Resolver;
using IdelPog.Console.Runtime.System;
using IdelPog.Console.Types;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Console.Mediator
{
    public class CommandResolverMediator : ICommandResolverMediator
    {
        private readonly IAssetRepository<Domain, IDomainResolver> _commandResolverMap;
        private readonly IDomainPermissionChecker _domainPermissionChecker;
        private readonly IFoundAssertion _foundAssertion;
        private readonly ISpanAssertion _spanAssertion;
        private readonly IDomainPermissionAssertion _domainPermissionAssertion;

        public CommandResolverMediator(IAssetRepository<Domain, IDomainResolver> commandResolverMap, IDomainPermissionChecker domainPermissionChecker,
            IFoundAssertion foundAssertion, ISpanAssertion spanAssertion, IDomainPermissionAssertion domainPermissionAssertion)
        {
            _commandResolverMap = commandResolverMap;
            _domainPermissionChecker = domainPermissionChecker;
            _foundAssertion = foundAssertion;
            _spanAssertion = spanAssertion;
            _domainPermissionAssertion = domainPermissionAssertion;
        }

        public void ResolveCommand(Domain domain, ReadOnlySpan<string> arguments)
        {
            _spanAssertion.AssertNotEmpty(arguments);
            _foundAssertion.AssertFound(domain, _commandResolverMap.Contains(domain));
            _domainPermissionAssertion.AssertHasPermission(_domainPermissionChecker.IsAllowed(domain), domain);

            IDomainResolver resolver = _commandResolverMap.Get(domain);

            resolver.Resolve(arguments);
        }
    }
}