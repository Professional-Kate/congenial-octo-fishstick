using Console.Commands.Domains;
using Console.Types;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions.Interfaces;

namespace Console.Commands.Resolver
{
    public class CommandResolverMediator : ICommandResolverMediator
    {
        private readonly IStateRepository<CommandDomain, ICommandDomainResolver> _commandResolverMap;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertFound _assertFound;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CommandResolverMediator(IStateRepository<CommandDomain, ICommandDomainResolver> commandResolverMap, IAssertNotNull assertNotNull, IAssertFound assertFound,  IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _commandResolverMap = commandResolverMap;
            _assertNotNull = assertNotNull;
            _assertFound = assertFound;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public void ResolveCommand(CommandDomain domain, string[] args)
        {
            _assertNotNull.AssertObjectNotNull(args);
            _assertCollectionNotEmpty.Handle(args);
            _assertFound.AssertItemIsFound(domain, () => _commandResolverMap.Contains(domain));
            
            ICommandDomainResolver commandResolver = _commandResolverMap.Get(domain);
            commandResolver.Resolve(args[0], args.Skip(1).ToArray());
        }
    }
}