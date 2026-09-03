using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Core.Service.Interface
{
    public interface IEnqueueEvent
    { 
        public void Enqueue(ScheduledCombatEvent scheduledCombatEvent);
    }
}