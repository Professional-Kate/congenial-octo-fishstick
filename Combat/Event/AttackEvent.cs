using IdelPog.Combat.Event.Interface;

namespace IdelPog.Combat.Event
{
    public readonly record struct AttackEvent : ICombatEvent
    {
        public EventType EventType => EventType.BASIC_ATTACK;
        public required double Tick { get; init; }
        public required byte AttackerID { get; init; }
    }
}