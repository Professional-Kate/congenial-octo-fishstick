using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct AbilityDamageComponent : IComponent
    {
        public required uint TotalDamage { get; init; }
    }
}