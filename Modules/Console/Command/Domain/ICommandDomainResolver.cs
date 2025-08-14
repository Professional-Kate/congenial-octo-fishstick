using IdelPog.Console.Types;

namespace IdelPog.Console.Command.Domain
{
    public interface ICommandDomainResolver
    {
        public Types.Domain HandledDomain { get; }
        public CommandDocumentation CommandDocumentation { get; }

        public void Resolve(ReadOnlySpan<string> arguments);
    }
}