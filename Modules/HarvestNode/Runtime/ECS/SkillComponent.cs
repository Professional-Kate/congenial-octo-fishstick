using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.HarvestNode.Runtime.ECS
{
    public readonly record struct SkillComponent : IComponent<SkillComponent>
    {
        public required SkillID SkillID { get; init; }
        
        public SkillComponent DeepClone()
        {
            return new  SkillComponent { SkillID = SkillID };
        }
    }
}