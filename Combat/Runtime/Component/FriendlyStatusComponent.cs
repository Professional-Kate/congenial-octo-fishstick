using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct FriendlyStatusComponent : IComponent
    {
        public required bool IsFriendly { get; init; }
    }
}