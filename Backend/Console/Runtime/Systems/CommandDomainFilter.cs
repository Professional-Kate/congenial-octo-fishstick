using Console.Runtime.ECS;
using Console.Types;
using IdelPog.ECS;
using IdelPog.ECS.Component;

namespace Console.Runtime.Systems
{
    public class CommandDomainFilter : ICommandDomainFilter
    {
        private readonly IEntity _allowedDomainEntity;

        public CommandDomainFilter(IEntity allowedDomainEntity)
        {
            _allowedDomainEntity = allowedDomainEntity;
        }

        public bool IsAllowed(CommandDomain domain)
        {
            ComponentStore<CommandDomainComponent> componentStore = _allowedDomainEntity.GetComponent<ComponentStore<CommandDomainComponent>>()
            CommandDomainComponent[] commandDomainComponents = componentStore.GetAllComponents();
            foreach (CommandDomainComponent commandDomainComponent in commandDomainComponents)
            {
                if (domain == commandDomainComponent.AllowedCommandDomain)
                {
                    return true;
                }
            }

            return false;
        }
    }
}