using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Event;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AttackScheduler : IAttackScheduler
    { 
        private readonly ICombatQueue _combatQueue;
        private readonly IDamageSystem _damageSystem;
        private readonly ICombatantRepository _combatantRepository;
        private readonly INumberAssertion _numberAssertion;

        public AttackScheduler(ICombatQueue combatQueue, IDamageSystem damageSystem, INumberAssertion numberAssertion, ICombatantRepository combatantRepository)
        {
            _combatQueue = combatQueue;
            _damageSystem = damageSystem;
            _numberAssertion = numberAssertion;
            _combatantRepository = combatantRepository;
        }

        public void EnqueueInitial(double tick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAll())
            { 
                CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
                
                _numberAssertion.AssertNumberNotZero(combatantStatsComponent.StatCard.Speed, combatantStatsComponent.StatCard.ToString());
            
                AttackEvent attackEvent = new(_damageSystem, combatantStatsComponent.StatCard, 0);
            
                double interval = 1.0 / combatantStatsComponent.StatCard.Speed;
                double nextTick = tick + interval;
            
                _combatQueue.Enqueue(attackEvent, nextTick);
            }
        }
    }
}