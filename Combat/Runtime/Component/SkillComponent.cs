using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct SkillComponent : IComponent<SkillComponent>
    {
        public required SkillType SkillType { get; init; }
        public required TargetingType TargetingType { get; init; }
        
        public SkillComponent DeepClone()
        {
            return this;
        }
    }
}