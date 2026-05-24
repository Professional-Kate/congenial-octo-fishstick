using IdelPog.Combat.Event;

namespace IdelPog.Combat.Service.Queue.Interface
{
    public interface ICombatQueue : IEnqueueEvent
    { 
        public CombatEvent Dequeue();
    }
}