using Console.Types;

namespace Console.Commands.Domains
{
    public interface ICommandDomainResolver
    {
        public CommandDomain HandledDomain { get; }

        public void Resolve(string[] args);
    }
}