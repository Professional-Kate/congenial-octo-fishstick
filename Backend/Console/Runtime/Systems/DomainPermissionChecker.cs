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
        private readonly IAssertComponentFound _assertComponentFound;

        public DomainPermissionChecker(IEntity allowedDomainEntity, IAssertComponentFound assertComponentFound)
        {
            _allowedDomainEntity = allowedDomainEntity;
            _assertComponentFound = assertComponentFound;
        }

        public bool IsAllowed(Domain domain)
        {
            bool contains = _allowedDomainEntity.TryGetComponent(out ComponentStore<DomainComponent> componentStore);
            _assertComponentFound.Handle(contains, typeof(ComponentStore<DomainComponent>));
            
            return componentStore.ContainsComponent(component => component.AllowedDomain == domain);
        }
    }
} 