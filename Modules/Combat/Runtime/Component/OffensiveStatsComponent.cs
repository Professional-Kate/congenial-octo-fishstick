using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct OffensiveStatsComponent : IComponent<OffensiveStatsComponent>
    {
        public required uint Attack { get; init; }
        
        public OffensiveStatsComponent DeepClone()
        {
            return this;
        }
    }
}