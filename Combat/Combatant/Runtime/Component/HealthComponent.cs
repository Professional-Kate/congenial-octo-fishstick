using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Runtime.Component
{
    public readonly record struct HealthComponent : IComponent
    { 
        public required uint Health { get; init; }
    }
}