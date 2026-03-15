using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event
{
    public interface ICombatEvent
    {
        public void RunEvent(IEnqueueEvent enqueueEvent, double tick);
    }
}