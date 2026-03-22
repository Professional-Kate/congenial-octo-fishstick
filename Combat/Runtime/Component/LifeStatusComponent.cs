using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct LifeStatusComponent : IComponent
    {
        public required bool IsAlive { get; init; }
    }
}