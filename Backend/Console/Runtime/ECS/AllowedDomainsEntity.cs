using Console.Types;
using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace Console.Runtime.ECS
{
    public record AllowedDomainsEntity : Entity
    {
        private readonly ComponentStore<CommandDomainComponent>  _componentStore;
        
        public AllowedDomainsEntity(CommandDomainComponent[] allowedDomains) : base(new ComponentStore<CommandDomainComponent>(allowedDomains, new ThrowHandler()))
        {
            _componentStore = GetComponent<ComponentStore<CommandDomainComponent>>();
        }

        public bool IsDomainAllowed(CommandDomain domain)
        {
            // TODO: doing this for every command is a little cringe. We need a Contains() or something to find a specific key 
            CommandDomainComponent[] commandDomainComponents = _componentStore.GetAllComponents();
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