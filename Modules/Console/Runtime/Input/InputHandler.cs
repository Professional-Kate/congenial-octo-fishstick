using IdelPog.Console.Argument.Interface;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Mediator;
using IdelPog.Console.Types;

namespace IdelPog.Console.Runtime.Input
{
    public sealed class InputHandler : IInputHandler
    {
        private readonly ICommandResolverMediator _commandResolverMediator;
        private readonly IArgumentResolver<Domain> _commandDomainResolver;
        private readonly ISpanAssertion _spanAssertion;

        public InputHandler(ICommandResolverMediator commandResolverMediator, IArgumentResolver<Domain> commandDomainResolver,
            ISpanAssertion spanAssertion)
        {
            _commandResolverMediator = commandResolverMediator;
            _commandDomainResolver = commandDomainResolver;
            _spanAssertion = spanAssertion;
        }

        public void Input(ReadOnlySpan<string> args)
        {
            _spanAssertion.AssertNotEmpty(args);
            Domain domain = _commandDomainResolver.Resolve(args[0]);

            ReadOnlySpan<string> commandArgs = args.Slice(1);
            _commandResolverMediator.ResolveCommand(domain, commandArgs);
        }
    }
}