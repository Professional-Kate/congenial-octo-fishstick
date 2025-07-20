using Console.Runtime.ECS;
using Console.Types;

namespace Console.Runtime.Factory
{
    public interface IDomainComponentFactory
    {
        public CommandDomainComponent CreateDomainComponent(CommandDomain commandDomain);
    }
}