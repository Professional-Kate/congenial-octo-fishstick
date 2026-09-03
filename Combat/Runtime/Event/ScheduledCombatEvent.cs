using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Runtime.Event
{
    public readonly record struct ScheduledCombatEvent
    {
        public required CombatEventType CombatEventType { get; init; }
        public required byte AbilityID { get; init; }
        public required byte AbilityStageIndex { get; init; }
        public required byte InstanceID { get; init; }
        public required TargetingType TargetingType { get; init; }
        public required double Tick { get; init; }
    }
}