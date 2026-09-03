using IdelPog.Combat.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct TriggerComponent : IComponent
    {
        public required TargetingType TargetingType { get; init; }
        public required TriggerEventType TriggerEventType { get; init; }
        public required uint MinTriggerValue { get; init; }
        public required uint MaxTriggerValue { get; init; }
    }
}