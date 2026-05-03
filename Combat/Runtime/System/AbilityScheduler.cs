using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AbilityScheduler : IBasicAttackScheduler
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly ICombatQueue _combatQueue;
        private readonly INumberAssertion _numberAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public AbilityScheduler(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatQueue combatQueue, INumberAssertion numberAssertion, IFoundAssertion foundAssertion)
        {
            _combatantRepository = combatantRepository;
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _combatQueue = combatQueue;
            _numberAssertion = numberAssertion;
            _foundAssertion = foundAssertion;
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
                    Enqueue(combatantAbilityEntity, tick, combatantEntity.GetComponent<CombatantStatsComponent>().Speed);
                }
            }
        }

        public void EnqueueAttack(double tick, byte attackerID, AbilityType abilityType)
        {
            _foundAssertion.AssertFound(attackerID, _combatantRepository.Contains(attackerID));
            _foundAssertion.AssertFound(attackerID, _combatantAbilityEntityRepository.Contains(attackerID));
            
            CombatantEntity combatantEntity = _combatantRepository.Get(attackerID);
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(attackerID, abilityType);
            
            Enqueue(combatantAbilityEntity, tick, combatantEntity.GetComponent<CombatantStatsComponent>().Speed);
        }
        
        private void Enqueue(CombatantAbilityEntity combatantAbilityEntity, double tick, uint combatantSpeed)
        {
            double abilityCooldown = combatantAbilityEntity.GetComponent<CooldownComponent>().Cooldown;
            
            _numberAssertion.AssertNumberNotZero(abilityCooldown, nameof(abilityCooldown));
            _numberAssertion.AssertNumberNotZero(combatantSpeed, nameof(combatantSpeed));
            
            // TODO: add a CastTime component to CombatantAbilityEntity. Change combatantSpeed to reduce CastTime instead.
            //  This won't be used here, but I'll need to change this when I made that change :)
            double nextTick = tick + abilityCooldown / combatantSpeed;
            
            // TODO: CastingEvent -> on dequeue just enqueues the BasicAttackEvent for Tick + CastingEvent.CastTime
            DirectDamageEvent directDamageEvent = new() { AttackerID = combatantAbilityEntity.CombatantID, Tick = nextTick, AbilityType = combatantAbilityEntity.AbilityType };
            _combatQueue.Enqueue(directDamageEvent, nextTick);
        }
    }
}