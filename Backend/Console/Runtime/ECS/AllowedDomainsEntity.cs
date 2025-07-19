using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace Console.Runtime.ECS
{
    public record AllowedDomainsEntity : Entity
    {
        public AllowedDomainsEntity(CommandDomainComponent[] allowedDomains) 
            : base(new ComponentStore<CommandDomainComponent>(allowedDomains, new ThrowHandler()))
        {
            
        }
    }
}