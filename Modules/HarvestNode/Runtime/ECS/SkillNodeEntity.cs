using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.HarvestNode.Runtime.ECS
{
    public sealed record SkillNodeEntity : Entity
    {
        public required SkillID SkillID { get; init; }
        
        private readonly ComponentStore<HarvestTargetComponent> _harvestTargetStore;
        
        public SkillNodeEntity(HarvestTargetComponent[] allowedNodes)
            : base(new ComponentStore<HarvestTargetComponent>(allowedNodes))
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