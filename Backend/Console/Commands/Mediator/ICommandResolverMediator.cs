using Console.Types;

namespace Console.Commands
{
    public interface ICommandResolverMediator
    {
        public void ResolveCommand(Domain domain, ReadOnlySpan<string> arguments);
    }
}