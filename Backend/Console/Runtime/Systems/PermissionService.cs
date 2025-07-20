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
        private readonly IDomainComponentFactory  _domainComponentFactory;
        private readonly IComponentStoreFactory _componentStoreFactory;
        private readonly IAssertComponentFound _assertComponentFound;
        private readonly IAssertComponentDoesNotExist _assertComponentDoesNotExist;

        public PermissionService(IEntity allowedDomainEntity, IDomainComponentFactory domainComponentFactory, IComponentStoreFactory componentStoreFactory, IAssertComponentFound assertComponentFound, IAssertComponentDoesNotExist assertComponentDoesNotExist)
        {
            _allowedDomainEntity = allowedDomainEntity;
            _domainComponentFactory = domainComponentFactory;
            _componentStoreFactory = componentStoreFactory;
            _assertComponentFound = assertComponentFound;
            _assertComponentDoesNotExist = assertComponentDoesNotExist;
        }

        public void PermissionUpdate(PermissionUpdateArguments arguments)
        {
            switch (arguments.ActionType)
            {
                case ActionType.ADD:
                    AddAllowedDomain(arguments.CommandDomain);
                    break;
                case ActionType.REMOVE:
                    RemoveAllowedDomain(arguments.CommandDomain);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void AddAllowedDomain(CommandDomain commandDomain)
        {
            ComponentStore<CommandDomainComponent> componentStore = TryGetComponentStore();
            _assertComponentDoesNotExist.Handle(componentStore.ContainsComponent(component => component.AllowedCommandDomain == commandDomain), typeof(CommandDomainComponent));

            CommandDomainComponent[] commandDomainComponents = componentStore.GetAllComponents();
            
            CommandDomainComponent[] newCommandDomainComponents = new CommandDomainComponent[commandDomainComponents.Length + 1];
            Array.Copy(commandDomainComponents, newCommandDomainComponents, commandDomainComponents.Length);
            newCommandDomainComponents[newCommandDomainComponents.Length] = _domainComponentFactory.CreateDomainComponent(commandDomain);
            
            _allowedDomainEntity.RemoveComponent<ComponentStore<CommandDomainComponent>>();
            _allowedDomainEntity.AddComponent(_componentStoreFactory.CreateComponentStore(newCommandDomainComponents));
        }

        private void RemoveAllowedDomain(CommandDomain commandDomain)
        {
            ComponentStore<CommandDomainComponent> componentStore = TryGetComponentStore();
            _assertComponentFound.Handle(componentStore.ContainsComponent(component => component.AllowedCommandDomain == commandDomain), typeof(CommandDomainComponent));
            
            CommandDomainComponent[] commandDomainComponents = componentStore.GetAllComponents();
            CommandDomainComponent[] newCommandDomainComponents = new CommandDomainComponent[commandDomainComponents.Length - 1];

            int writeIndex = 0;
            foreach (CommandDomainComponent component in commandDomainComponents)
            {
                if (component.AllowedCommandDomain == commandDomain)
                {
                    // skipping the component with matching ID
                    continue;
                }

                newCommandDomainComponents[writeIndex] = component;
                writeIndex++;
            }
            
            _allowedDomainEntity.RemoveComponent<ComponentStore<CommandDomainComponent>>();
            _allowedDomainEntity.AddComponent(_componentStoreFactory.CreateComponentStore(newCommandDomainComponents));
        }

        private ComponentStore<CommandDomainComponent> TryGetComponentStore()
        {
            bool contains = _allowedDomainEntity.TryGetComponent(out ComponentStore<CommandDomainComponent> componentStore);
            _assertComponentFound.Handle(contains, typeof(ComponentStore<CommandDomainComponent>));
            
            return  componentStore;
        }
    }
}