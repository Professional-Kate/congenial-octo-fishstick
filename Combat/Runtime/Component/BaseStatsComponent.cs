using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct BaseStatsComponent : IComponent
    {
        public required uint Health { get; init; }
        
        public HealthComponent Stats => new() { Health = Health };
    }
}