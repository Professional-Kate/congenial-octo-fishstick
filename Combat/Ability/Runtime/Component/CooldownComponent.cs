using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct CooldownComponent : IComponent
    {
        public required double Cooldown { get; init; }
    }
}