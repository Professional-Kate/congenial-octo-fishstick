using IdelPog.Console.Types;

namespace IdelPog.Console.Mediator
{
    public interface ICommandResolverMediator
    {
        public void ResolveCommand(Domain domain, ReadOnlySpan<string> arguments);
    }
}