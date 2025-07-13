using Console.Commands.Domains;
using Console.Types;
using IdelPog.Common.Repository;

namespace Console.Commands.Resolver
{
    public class CommandResolverMediator : ICommandResolverMediator
    {
        private readonly IStateRepository<CommandDomain, ICommandDomainResolver> _commandResolverMap;

        public CommandResolverMediator(IStateRepository<CommandDomain, ICommandDomainResolver> commandResolverMap)
        {
            _commandResolverMap = commandResolverMap;
        }
        
        public void ResolveCommand(CommandDomain domain, string[] args)
        {
            throw new NotImplementedException();
        }
    }
}