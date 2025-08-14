using IdelPog.Console.Runtime.ECS;
using IdelPog.Console.Types;

namespace IdelPog.Console.Factory.Interface
{
    public interface IDomainComponentFactory
    {
        public DomainComponent CreateDomainComponent(Domain domain);
    }
}