using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct ReadyTickComponent : IComponent
    {
        public required double ReadyTick { get; init; }
    }
}