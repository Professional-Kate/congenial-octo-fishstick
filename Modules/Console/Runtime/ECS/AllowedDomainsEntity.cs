using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.Console.Runtime.ECS
{
    public sealed record AllowedDomainsEntity : Entity
    {
        public AllowedDomainsEntity(DomainComponent[] allowedDomains)
            : base(new ComponentStore<DomainComponent>(allowedDomains))
        {
        }
    }
}