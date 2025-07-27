using IdelPog.Common.Enums;
using IdelPog.ECS.Component;

namespace ContentEngine.Runtime.ECS
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