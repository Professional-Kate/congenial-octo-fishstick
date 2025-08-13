using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;

namespace IdelPog.HarvestNode.Runtime.ECS
{
    public sealed record SkillNodeEntity : Entity
    {
        private readonly ComponentStore<ResourceComponent> _resourceStore;
        
        public SkillNodeEntity(SkillComponent skillComponent, ResourceComponent[] allowedNodes)
            : base(skillComponent, new ComponentStore<ResourceComponent>(allowedNodes, new ThrowHandler()))
        {
            _resourceStore = GetComponent<ComponentStore<ResourceComponent>>();
        }

        public bool Allows(ResourceID resourceID)
        {
            bool contains =_resourceStore.ContainsComponent(resource => resource.ResourceID == resourceID);
            return contains;
        }
    }
}