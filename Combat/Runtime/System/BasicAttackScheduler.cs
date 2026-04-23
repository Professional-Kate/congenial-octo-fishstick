using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Event;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class BasicAttackScheduler : IBasicAttackScheduler
    { 
        private readonly ICombatQueue _combatQueue;
        private readonly ICombatantRepository _combatantRepository;
        private readonly INumberAssertion _numberAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public BasicAttackScheduler(ICombatQueue combatQueue, INumberAssertion numberAssertion, ICombatantRepository combatantRepository, IFoundAssertion foundAssertion)
        {
            _combatQueue = combatQueue;
            _numberAssertion = numberAssertion;
            _combatantRepository = combatantRepository;
            _foundAssertion = foundAssertion;
        }

        public void EnqueueInitial(double tick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAll())
            {
                ComponentStore<SkillComponent> componentStore = combatantEntity.GetComponent<ComponentStore<SkillComponent>>();
                if (componentStore.ContainsComponent(component => component.SkillType == SkillType.BASIC_ATTACK))
                { 
                    Enqueue(combatantEntity, tick);
                }
            }
        }

        public void EnqueueAttack(double tick, byte attackerID)
        {
            _foundAssertion.AssertFound(attackerID, _combatantRepository.Contains(attackerID));
            
            CombatantEntity combatantEntity = _combatantRepository.Get(attackerID);
            
            Enqueue(combatantEntity, tick);
        }

        private void Enqueue(CombatantEntity combatantEntity, double tick)
        {
            CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            
            _numberAssertion.AssertNumberNotZero(combatantStatsComponent.Speed, combatantStatsComponent.ToString());
            
            double interval = 1.0 / combatantStatsComponent.Speed;
            double nextTick = tick + interval;
            
            BasicAttackEvent basicAttackEvent = new() { AttackerID = combatantEntity.CombatantID, Tick =  nextTick };
            _combatQueue.Enqueue(basicAttackEvent, nextTick);
        }
    }
}