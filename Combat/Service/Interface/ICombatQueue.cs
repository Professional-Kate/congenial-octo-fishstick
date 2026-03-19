using IdelPog.Combat.Event.Interface;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatQueue : IEnqueueEvent
    { 
        public ICombatEvent Dequeue();
    }
}