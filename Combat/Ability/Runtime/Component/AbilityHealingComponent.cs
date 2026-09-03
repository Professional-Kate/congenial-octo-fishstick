using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct AbilityHealingComponent : IComponent
    {
        public required uint TotalHealing { get; init; }
    }
}