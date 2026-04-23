using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct DamageComponent : IComponent
    {
        public required uint Damage { get; init; }
    }
}