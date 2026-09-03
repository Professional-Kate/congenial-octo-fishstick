using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Contracts.Card
{
    public readonly record struct TriggerCard
    { 
        public required TargetingType TargetingType { get; init; }
        public required TriggerEventType TriggerEventType { get; init; }
        public required uint MinTriggerValue { get; init; }
        public required uint MaxTriggerValue { get; init; }
    }
}