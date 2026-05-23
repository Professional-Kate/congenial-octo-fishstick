using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct CastTimeComponent : IComponent
    {
        public required double CastTime { get; init; }
    }
}