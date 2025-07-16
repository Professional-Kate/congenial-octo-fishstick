using Console.Types;

namespace Console.Commands.Domains
{
    public interface ICommandDomainResolver
    {
        public CommandDomain HandledDomain { get; }
        public CommandDocumentation CommandDocumentation { get; }

        public void Resolve(string[] arguments);
    }
}