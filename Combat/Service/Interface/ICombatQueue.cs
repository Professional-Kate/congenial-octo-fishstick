using IdelPog.Combat.Event;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatQueue : IEnqueueEvent
    { 
        public CombatEvent Dequeue();
    }
}