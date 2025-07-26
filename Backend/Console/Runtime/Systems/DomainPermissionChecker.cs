using Console.Runtime.ECS;
using Console.Types;
using IdelPog.ECS;
using IdelPog.ECS.Assertions;
using IdelPog.ECS.Component;

namespace Console.Runtime.Systems
{
    public class DomainPermissionChecker : IDomainPermissionChecker
    {
        private readonly IEntity _allowedDomainEntity;
        private readonly IComponentAssertion _componentAssertion;

        public DomainPermissionChecker(IEntity allowedDomainEntity, IComponentAssertion componentAssertion)
        {
            _allowedDomainEntity = allowedDomainEntity;
            _componentAssertion = componentAssertion;
        }

        public bool IsAllowed(Domain domain)
        {
            bool contains = _allowedDomainEntity.TryGetComponent(out ComponentStore<DomainComponent> componentStore);
            _componentAssertion.AssertFound<ComponentStore<DomainComponent>>(contains);

            return componentStore.ContainsComponent(component => component.AllowedDomain == domain);
        }
    }
}