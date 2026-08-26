using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct AbilityHealingComponent : IComponent
    {
        public required uint TotalHealing { get; init; }
    }
}