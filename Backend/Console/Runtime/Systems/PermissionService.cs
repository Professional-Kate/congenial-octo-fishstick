using Console.Commands.Domains.Arguments;
using Console.Runtime.ECS;
using Console.Runtime.Factory;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.ECS;
using IdelPog.ECS.Assertions;
using IdelPog.ECS.Component;
using IdelPog.ECS.Factory;

namespace Console.Runtime.Systems
{
    public class PermissionService : IPermissionService
    {
        private readonly IEntity _allowedDomainEntity;
        private readonly IDomainComponentFactory _domainComponentFactory;
        private readonly IComponentStoreFactory _componentStoreFactory;
        private readonly IComponentAssertion _componentAssertion;

        public PermissionService(IEntity allowedDomainEntity, IDomainComponentFactory domainComponentFactory, IComponentStoreFactory componentStoreFactory,
            IComponentAssertion componentAssertion)
        {
            _allowedDomainEntity = allowedDomainEntity;
            _domainComponentFactory = domainComponentFactory;
            _componentStoreFactory = componentStoreFactory;
            _componentAssertion = componentAssertion;
        }

        public void PermissionUpdate(PermissionUpdateArguments arguments)
        {
            switch (arguments.ActionType)
            {
                case ActionType.ADD:
                    AddAllowedDomain(arguments.Domain);
                    break;
                case ActionType.REMOVE:
                    RemoveAllowedDomain(arguments.Domain);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddAllowedDomain(Domain domain)
        {
            ComponentStore<DomainComponent> componentStore = TryGetComponentStore();
            _componentAssertion.AssertUnique<DomainComponent>(componentStore.ContainsComponent(component => component.AllowedDomain == domain));

            DomainComponent[] commandDomainComponents = componentStore.GetAllComponents();

            DomainComponent[] newCommandDomainComponents = new DomainComponent[commandDomainComponents.Length + 1];
            Array.Copy(commandDomainComponents, newCommandDomainComponents, commandDomainComponents.Length);
            newCommandDomainComponents[^1] = _domainComponentFactory.CreateDomainComponent(domain);

            _allowedDomainEntity.RemoveComponent<ComponentStore<DomainComponent>>();
            _allowedDomainEntity.AddComponent(_componentStoreFactory.CreateComponentStore(newCommandDomainComponents));
        }

        private void RemoveAllowedDomain(Domain domain)
        {
            ComponentStore<DomainComponent> componentStore = TryGetComponentStore();
            _componentAssertion.AssertFound<DomainComponent>(componentStore.ContainsComponent(component => component.AllowedDomain == domain));

            DomainComponent[] commandDomainComponents = componentStore.GetAllComponents();
            DomainComponent[] newCommandDomainComponents = new DomainComponent[commandDomainComponents.Length - 1];

            int writeIndex = 0;
            foreach (DomainComponent component in commandDomainComponents)
            {
                if (component.AllowedDomain == domain)
                {
                    // skipping the component with matching ID
                    continue;
                }

                newCommandDomainComponents[writeIndex] = component;
                writeIndex++;
            }

            _allowedDomainEntity.RemoveComponent<ComponentStore<DomainComponent>>();
            _allowedDomainEntity.AddComponent(_componentStoreFactory.CreateComponentStore(newCommandDomainComponents));
        }

        private ComponentStore<DomainComponent> TryGetComponentStore()
        {
            bool contains = _allowedDomainEntity.TryGetComponent(out ComponentStore<DomainComponent> componentStore);
            _componentAssertion.AssertFound<ComponentStore<DomainComponent>>(contains);

            return componentStore;
        }
    }
}