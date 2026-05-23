using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Event
{
    public readonly record struct CombatEvent
    {
        public required EventType EventType { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required byte AttackerID { get; init; }
        public required double Tick { get; init; }
    }
}