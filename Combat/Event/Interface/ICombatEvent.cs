using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Event.Interface
{
    public interface ICombatEvent
    {
        public EventType EventType { get; }
        public AbilityType AbilityType { get; }
        public byte AttackerID { get; }
        public double Tick { get; }
    }
}