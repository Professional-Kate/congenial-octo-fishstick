using Console.Commands;
using Console.Commands.Resolver;
using Console.Runtime.Input.Assertions;
using Console.Types;

namespace Console.Runtime.Input
{
    public sealed class InputHandler : IInputHandler
    {
        private readonly ICommandResolverMediator _commandResolverMediator;
        private readonly IArgumentResolver<CommandDomain>  _commandDomainResolver;
        private readonly IAssertSpanNotEmpty  _assertSpanNotEmpty;

        public InputHandler(ICommandResolverMediator commandResolverMediator, IArgumentResolver<CommandDomain> commandDomainResolver, IAssertSpanNotEmpty  assertSpanNotEmpty)
        {
            _commandResolverMediator = commandResolverMediator;
            _commandDomainResolver = commandDomainResolver;
            _assertSpanNotEmpty = assertSpanNotEmpty;
        }

        public void Input(ReadOnlySpan<string> args)
        {
            _assertSpanNotEmpty.Handle(args);
            CommandDomain commandDomain = _commandDomainResolver.Resolve(args[0]);

            ReadOnlySpan<string> commandArgs = args.Slice(1);
            _commandResolverMediator.ResolveCommand(commandDomain, commandArgs);
        }
    }
}