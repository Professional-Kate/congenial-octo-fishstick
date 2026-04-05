using IdelPog.Combat.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct TargetingTypeComponent : IComponent
    {
        public required TargetingType TargetingType { get; init; }
    }
}