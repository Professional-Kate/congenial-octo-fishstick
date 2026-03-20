namespace IdelPog.Combat.Event.Interface
{
    public interface ICombatEvent
    {
        public EventType EventType { get; }
        public byte AttackerID { get; }
        public double Tick { get; }
    }
}