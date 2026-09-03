using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Runtime.Component
{
    public readonly record struct RetaliationComponent : IComponent
    {
        private readonly Queue<CombatantDamageComponent> _damageQueue;

        public required byte Capacity { get; init; }
        public int Count => _damageQueue.Count;

        public RetaliationComponent()
        {
            _damageQueue = new Queue<CombatantDamageComponent>(Capacity);
        }

        public void Enqueue(CombatantDamageComponent combatantDamageComponent)
        {
            if (_damageQueue.Count >= Capacity)
            {
                _damageQueue.Dequeue();
            }
            
            _damageQueue.Enqueue(combatantDamageComponent);
        }

        public bool TryDequeue(out CombatantDamageComponent combatantDamageComponent)
        {
            if (_damageQueue.Count == 0)
            {
                combatantDamageComponent = default;
                return false;
            }
            
            combatantDamageComponent = _damageQueue.Dequeue();
            return true;
        }
    }
}