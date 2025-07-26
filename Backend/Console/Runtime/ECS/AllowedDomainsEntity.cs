using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace Console.Runtime.ECS
{
    public record AllowedDomainsEntity : Entity
    {
        public AllowedDomainsEntity(DomainComponent[] allowedDomains)
            : base(new ComponentStore<DomainComponent>(allowedDomains, new ThrowHandler()))
        {
        }
    }
}