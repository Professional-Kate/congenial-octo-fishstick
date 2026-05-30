using IdelPog.Combat.Event;
using IdelPog.Combat.Service.Queue.Interface;

namespace IdelPog.Combat.Service.Queue
{
    public sealed class CombatQueue : ICombatQueue, ICombatQueueClear
    {
        private readonly PriorityQueue<CombatEvent, double> _combatQueue = new();
        
        public void Enqueue(CombatEvent combatEvent) => _combatQueue.Enqueue(combatEvent, combatEvent.Tick);
        
        public CombatEvent Dequeue() => _combatQueue.Dequeue();
        
        public void Clear() => _combatQueue.Clear();
    }
}