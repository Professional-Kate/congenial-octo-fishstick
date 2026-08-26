using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct BaseHealthComponent : IComponent
    {
        public required uint Health { get; init; }
    }
}