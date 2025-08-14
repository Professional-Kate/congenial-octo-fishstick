using IdelPog.Console.Factory.Interface;
using IdelPog.Console.Runtime.ECS;
using IdelPog.Console.Types;

namespace IdelPog.Console.Factory
{
    public class DomainComponentFactory : IDomainComponentFactory
    {
        public DomainComponent CreateDomainComponent(Domain domain)
        {
            return new DomainComponent { AllowedDomain = domain };
        }
    }
}