using Console.Commands.Domains;
using Console.Types;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions.Interfaces;

namespace Console.Commands.Resolver
{
    public class CommandResolverMediator : ICommandResolverMediator
    {
        private readonly IStateRepository<CommandDomain, ICommandDomainResolver> _commandResolverMap;
        private readonly IAssertFound _assertFound;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CommandResolverMediator(IStateRepository<CommandDomain, ICommandDomainResolver> commandResolverMap, IAssertFound assertFound,  IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _commandResolverMap = commandResolverMap;
            _assertFound = assertFound;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public void ResolveCommand(CommandDomain domain, string[] args)
        {
            _assertCollectionNotEmpty.Handle(args);
            _assertFound.AssertItemIsFound(domain, () => _commandResolverMap.Contains(domain));
            
            ICommandDomainResolver commandResolver = _commandResolverMap.Get(domain);
            commandResolver.Resolve(args[0], args.Skip(1).ToArray());
        }
    }
}