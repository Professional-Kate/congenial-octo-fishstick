using Console.Types;

namespace Console.Commands.Domains
{
    public interface ICommandDomainResolver
    {
        public Domain HandledDomain { get; }
        public CommandDocumentation CommandDocumentation { get; }

        public void Resolve(ReadOnlySpan<string> arguments);
    }
}