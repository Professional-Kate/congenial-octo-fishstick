using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct SpeedComponent : IComponent
    {
        public required uint Speed { get; init; }
    }
}