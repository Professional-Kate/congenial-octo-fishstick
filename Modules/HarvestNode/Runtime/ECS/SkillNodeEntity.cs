using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asserter;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.HarvestNode.Runtime.ECS
{
    public sealed record SkillNodeEntity : Entity
    {
        public required SkillID SkillID { get; init; }
        
        private readonly ComponentStore<HarvestTargetComponent> _harvestTargetStore;
        
        public SkillNodeEntity(IRepositoryAsserter repositoryAsserter, HarvestTargetComponent[] allowedNodes)
            : base(repositoryAsserter, new ComponentStore<HarvestTargetComponent>(allowedNodes))
        {
            _harvestTargetStore = GetComponent<ComponentStore<HarvestTargetComponent>>();
        }

        public bool Allows(ResourceID harvestTarget)
        {
            bool contains =_harvestTargetStore.ContainsComponent(resource => resource.HarvestTarget == harvestTarget);
            return contains;
        }
    }
}