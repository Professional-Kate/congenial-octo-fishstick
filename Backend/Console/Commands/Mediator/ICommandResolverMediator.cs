using Console.Types;

namespace Console.Commands
{
    public interface ICommandResolverMediator
    {
        public void ResolveCommand(CommandDomain domain, string[] arguments);
    }
}