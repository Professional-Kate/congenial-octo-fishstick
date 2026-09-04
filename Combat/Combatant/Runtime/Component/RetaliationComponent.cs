using IdelPog.Combat.Combatant.Contracts;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Runtime.Component
{
    public readonly record struct RetaliationComponent : IComponent
    {
        private readonly Queue<CombatantDamaged> _damageQueue;

        public required byte Capacity { get; init; }
        public int Count => _damageQueue.Count;

        public RetaliationComponent()
        {
            _damageQueue = new Queue<CombatantDamaged>(Capacity);
        }

        public void Enqueue(CombatantDamaged combatantDamaged)
        {
            if (_damageQueue.Count >= Capacity)
            {
                _damageQueue.Dequeue();
            }
            
            _damageQueue.Enqueue(combatantDamaged);
        }

        public bool TryDequeue(out CombatantDamaged combatantDamaged)
        {
            if (_damageQueue.Count == 0)
            {
                combatantDamaged = default;
                return false;
            }
            
            combatantDamaged = _damageQueue.Dequeue();
            return true;
        }
    }
}