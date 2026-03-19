using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Event;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AttackScheduler : IAttackScheduler
    { 
        private readonly ICombatQueue _combatQueue;
        private readonly IDamageSystem _damageSystem;
        private readonly ICombatantRepository _combatantRepository;
        private readonly INumberAssertion _numberAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public AttackScheduler(ICombatQueue combatQueue, IDamageSystem damageSystem, INumberAssertion numberAssertion, ICombatantRepository combatantRepository, IFoundAssertion foundAssertion)
        {
            _combatQueue = combatQueue;
            _damageSystem = damageSystem;
            _numberAssertion = numberAssertion;
            _combatantRepository = combatantRepository;
            _foundAssertion = foundAssertion;
        }

        public void EnqueueInitial(double tick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAll())
            { 
                Enqueue(combatantEntity, tick);
            }
        }

        public void EnqueueAttack(double tick, byte id)
        {
            _foundAssertion.AssertFound(id, _combatantRepository.Contains(id));
            
            CombatantEntity combatantEntity = _combatantRepository.Get(id);
            
            Enqueue(combatantEntity, tick);
        }

        private void Enqueue(CombatantEntity combatantEntity, double tick)
        {
            CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            
            _numberAssertion.AssertNumberNotZero(combatantStatsComponent.StatCard.Speed, combatantStatsComponent.StatCard.ToString());
            
            double interval = 1.0 / combatantStatsComponent.StatCard.Speed;
            double nextTick = tick + interval;
            
            AttackEvent attackEvent = new(_damageSystem, nextTick, combatantEntity.CombatantID);
            _combatQueue.Enqueue(attackEvent, nextTick);
        }
    }
}