using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class InitialAbilityScheduler : IInitialAbilityScheduler
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly INumberAssertion _numberAssertion;

        public InitialAbilityScheduler(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IAbilityEventScheduler abilityEventScheduler, INumberAssertion numberAssertion)
        {
            _combatantRepository = combatantRepository;
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _abilityEventScheduler = abilityEventScheduler;
            _numberAssertion = numberAssertion;
        }

        public void EnqueueInitial(double tick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAll())
            {
                // if no Abilities have been created for CombatantID, then we have nothing to enqueue
                if (_combatantAbilityEntityRepository.Contains(combatantEntity.CombatantID) == false)
                {
                    continue;
                } 
                
                IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityRepository.GetAll(combatantEntity.CombatantID);
                foreach (CombatantAbilityEntity combatantAbilityEntity in combatantAbilityEntities)
                { 
                    // TODO: update with Initiative
                    _abilityEventScheduler.ScheduleEvent(tick - GetCombatantSpeed(combatantEntity), combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType);
                }
            }
        }

        private uint GetCombatantSpeed(CombatantEntity combatantEntity)
        { 
            CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            _numberAssertion.AssertNumberNotZero(combatantStatsComponent.Speed, nameof(combatantStatsComponent.Speed));

            return combatantStatsComponent.Speed;
        }
    }   
}