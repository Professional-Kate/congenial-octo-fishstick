using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct HealthComponent : IComponent
    { 
        public required uint Health { get; init; }
    }
}