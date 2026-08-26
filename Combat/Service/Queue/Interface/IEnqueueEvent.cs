using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Service.Queue.Interface
{
    public interface IEnqueueEvent
    { 
        public void Enqueue(ScheduledCombatEvent scheduledCombatEvent);
    }
}