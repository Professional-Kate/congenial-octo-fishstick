namespace IdelPog.Console.Resolver
{
    public interface IDomainResolver
    {
        public void Resolve(ReadOnlySpan<string> arguments);
    }
}