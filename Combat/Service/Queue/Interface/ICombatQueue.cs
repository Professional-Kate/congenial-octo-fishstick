using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Service.Queue.Interface
{
    public interface ICombatQueue : IEnqueueEvent
    { 
        public ScheduledCombatEvent Dequeue();
    }
}