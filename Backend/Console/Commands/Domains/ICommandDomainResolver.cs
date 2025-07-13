namespace Console.Commands.Domains
{
    public interface ICommandDomainResolver
    {
        public string HandledDomainName { get; }

        public void Resolve(string action, string[] args);
    }
}