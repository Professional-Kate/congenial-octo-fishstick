using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct AgilityComponent : IComponent
    {
        public required uint Speed { get; init; }
        public required uint Initiative { get; init; }
    }
}