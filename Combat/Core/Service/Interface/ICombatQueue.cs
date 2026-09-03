using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Core.Service.Interface
{
    public interface ICombatQueue : IEnqueueEvent
    { 
        public ScheduledCombatEvent Dequeue();
    }
}