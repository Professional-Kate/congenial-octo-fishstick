using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver
{
    public interface ICommandDomainResolver
    {
        public Domain HandledDomain { get; }

        public void Resolve(ReadOnlySpan<string> arguments);
    }
}