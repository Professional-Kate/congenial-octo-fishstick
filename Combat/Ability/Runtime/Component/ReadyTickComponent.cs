using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct ReadyTickComponent : IComponent
    {
        public required double ReadyTick { get; init; }
    }
}