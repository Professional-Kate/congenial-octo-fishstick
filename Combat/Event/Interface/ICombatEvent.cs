namespace IdelPog.Combat.Event.Interface
{
    public interface ICombatEvent
    {
        public EventType EventType { get; }
        public double Tick { get; }
    }
}