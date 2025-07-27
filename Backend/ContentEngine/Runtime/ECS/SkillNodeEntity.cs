using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentEngine.Runtime.ECS
{
    public sealed record SkillNodeEntity : Entity
    {
        public SkillNodeEntity(SkillComponent skillComponent, ResourceComponent[] allowedNodes)
            : base(skillComponent, new ComponentStore<ResourceComponent>(allowedNodes, new ThrowHandler()))
        { 
        }
    }
}