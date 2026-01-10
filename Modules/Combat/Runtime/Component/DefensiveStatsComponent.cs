using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct DefensiveStatsComponent : IComponent<DefensiveStatsComponent>
    {
        public required uint Health { get; init; }
        
        public DefensiveStatsComponent DeepClone()
        {
            return this;
        }
    }
}