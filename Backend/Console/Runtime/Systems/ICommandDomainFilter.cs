using Console.Types;

namespace Console.Runtime.Systems
{
    public interface ICommandDomainFilter
    {
        public bool IsAllowed(CommandDomain domain);
    }
}