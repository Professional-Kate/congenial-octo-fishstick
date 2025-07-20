using Console.Runtime.ECS;
using Console.Types;

namespace Console.Runtime.Factory
{
    public class DomainComponentFactory : IDomainComponentFactory
    {
        public CommandDomainComponent CreateDomainComponent(CommandDomain commandDomain)
        {
            return new  CommandDomainComponent { AllowedCommandDomain = commandDomain };
        }
    }
}