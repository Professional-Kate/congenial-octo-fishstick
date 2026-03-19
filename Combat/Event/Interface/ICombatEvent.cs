using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event.Interface
{
    public interface ICombatEvent
    {
        public double Tick { get; }

        public void RunEvent(IEnqueueEvent enqueueEvent);
    }
}