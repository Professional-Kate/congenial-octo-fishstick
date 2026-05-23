using IdelPog.Combat.Event;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatQueue : ICombatQueue
    {
        private readonly PriorityQueue<CombatEvent, double> _combatQueue = new();
        
        public void Enqueue(CombatEvent combatEvent, double tick) => _combatQueue.Enqueue(combatEvent, tick);
        
        public CombatEvent Dequeue() => _combatQueue.Dequeue();
    }
}