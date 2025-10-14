using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.Console.Runtime.ECS
{
    public sealed record AllowedDomainsEntity : Entity
    {
        public AllowedDomainsEntity(IRepositoryAsserter repositoryAsserter, DomainComponent[] allowedDomains)
            : base(repositoryAsserter, new ComponentStore<DomainComponent>(allowedDomains))
        {
        }
    }
}