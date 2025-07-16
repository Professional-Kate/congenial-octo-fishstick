using Console.Commands;
using Console.Commands.Resolver;
using Console.Types;

namespace Console.Runtime.Input
{
    public sealed class InputHandler : IInputHandler
    {
        private readonly ICommandResolverMediator _commandResolverMediator;
        private readonly IArgumentResolver<CommandDomain>  _commandDomainResolver;

        public InputHandler(ICommandResolverMediator commandResolverMediator, IArgumentResolver<CommandDomain> commandDomainResolver)
        {
            _commandResolverMediator = commandResolverMediator;
            _commandDomainResolver = commandDomainResolver;
        }

        public void Input(ReadOnlySpan<string> args)
        {
            // TODO: check span size 
            CommandDomain commandDomain = _commandDomainResolver.Resolve(args[0]);

            ReadOnlySpan<string> commandArgs = args.Slice(1);
            _commandResolverMediator.ResolveCommand(commandDomain, commandArgs);
        }
    }
}