using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct UtilityStatsComponent : IComponent<UtilityStatsComponent>
    {
        public required uint Speed { get; init; }
        
        public UtilityStatsComponent DeepClone()
        {
            return this;
        }
    }
}