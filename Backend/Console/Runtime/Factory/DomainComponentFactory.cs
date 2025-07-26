using Console.Runtime.ECS;
using Console.Types;

namespace Console.Runtime.Factory
{
    public class DomainComponentFactory : IDomainComponentFactory
    {
        public DomainComponent CreateDomainComponent(Domain domain)
        {
            return new DomainComponent { AllowedDomain = domain };
        }
    }
}