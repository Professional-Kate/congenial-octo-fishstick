using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.Console.Runtime.ECS
{
    public record AllowedDomainsEntity : Entity
    {
        public AllowedDomainsEntity(DomainComponent[] allowedDomains)
            : base(new ComponentStore<DomainComponent>(allowedDomains, new ThrowHandler()))
        {
        }
    }
}