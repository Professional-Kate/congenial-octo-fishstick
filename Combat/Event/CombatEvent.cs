using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Event
{
    public readonly record struct CombatEvent
    {
        public required EventType EventType { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required byte CombatantID { get; init; }
        public required double Tick { get; init; }
    }
}