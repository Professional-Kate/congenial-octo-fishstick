using IdelPog.Combat.Event;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatQueue : ICombatQueue
    {
        private readonly PriorityQueue<ICombatEvent, double> _combatQueue = new();
        
        public void Enqueue(ICombatEvent combatEvent, double time) => _combatQueue.Enqueue(combatEvent, time);
    }
}