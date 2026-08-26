using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct AbilityDamageComponent : IComponent
    {
        public required uint TotalDamage { get; init; }
    }
}