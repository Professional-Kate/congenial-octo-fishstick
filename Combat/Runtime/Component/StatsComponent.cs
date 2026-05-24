using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct StatsComponent : IComponent
    { 
        public required uint Attack { get; init; }
        public required uint Health { get; init; }
    }
}