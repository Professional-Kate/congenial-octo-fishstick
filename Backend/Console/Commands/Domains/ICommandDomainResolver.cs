using Console.Types;

namespace Console.Commands.Domains
{
    public interface ICommandDomainResolver
    {
        public CommandDomain HandledPermission { get; }
        public CommandDocumentation CommandDocumentation { get; }

        public void Resolve(ReadOnlySpan<string> arguments);
    }
}