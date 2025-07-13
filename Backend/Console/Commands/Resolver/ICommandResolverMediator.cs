using Console.Types;

namespace Console.Commands.Resolver
{
    public interface ICommandResolverMediator
    {
        public void ResolveCommand(CommandDomain domain, string[] args);
    }
}