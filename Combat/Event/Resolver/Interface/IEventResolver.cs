namespace IdelPog.Combat.Event.Resolver.Interface
{
    public interface IEventResolver
    {
        public EventType EventType { get; }

        public void ResolveEvent(double tick, byte combatantID);
    }
}