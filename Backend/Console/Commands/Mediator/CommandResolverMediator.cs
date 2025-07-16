using Console.Assertions;
using Console.Commands.Domains;
using Console.Types;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;

namespace Console.Commands
{
    public class CommandResolverMediator : ICommandResolverMediator
    {
        private readonly IStateRepository<CommandDomain, ICommandDomainResolver> _commandResolverMap;
        private readonly IAssertFound _assertFound;
        private readonly IAssertSpanNotEmpty _assertSpanNotEmpty;

        public CommandResolverMediator(IStateRepository<CommandDomain, ICommandDomainResolver> commandResolverMap, IAssertFound assertFound,  IAssertSpanNotEmpty assertSpanNotEmpty)
        {
            _commandResolverMap = commandResolverMap;
            _assertFound = assertFound;
            _assertSpanNotEmpty = assertSpanNotEmpty;
        }
        
        public void ResolveCommand(CommandDomain domain, ReadOnlySpan<string> arguments)
        {
            _assertSpanNotEmpty.Handle(arguments);
            _assertFound.AssertItemIsFound(domain, () => _commandResolverMap.Contains(domain));
            
            ICommandDomainResolver commandResolver = _commandResolverMap.Get(domain);
            commandResolver.Resolve(arguments);
        }
    }
}