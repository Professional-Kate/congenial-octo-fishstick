namespace IdelPog.Console.Resolver
{
    public interface ISubDomainResolver
    { 
        public void Resolve(ReadOnlySpan<string> arguments);

        public string GetHelp();
    }
}