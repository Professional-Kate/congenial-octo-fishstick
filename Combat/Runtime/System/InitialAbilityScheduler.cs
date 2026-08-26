using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class InitialAbilityScheduler : IInitialAbilityScheduler
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly ICombatantAbilityInitializer _combatantAbilityInitializer;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly ITriggerSubscriber _triggerSubscriber;

        public InitialAbilityScheduler(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatantAbilityInitializer combatantAbilityInitializer, IAbilityEventScheduler abilityEventScheduler, ITriggerSubscriber triggerSubscriber)
        {
            _combatantRepository = combatantRepository;
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _combatantAbilityInitializer = combatantAbilityInitializer;
            _abilityEventScheduler = abilityEventScheduler;
            _triggerSubscriber = triggerSubscriber;
        }

        public void ScheduleRegisteredAbilities(double initialTick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.GetAllParticipating())
            {
                // if no Abilities have been created for CombatantID, then we have nothing to enqueue
                if (_combatantAbilityEntityRepository.Contains(combatantEntity.CombatantID) == false)
                {
                    continue;
                } 
                
                double readyTime = initialTick - GetCombatantInitiative(combatantEntity);
                
                IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityRepository.GetAll(combatantEntity.CombatantID);
                _combatantAbilityInitializer.InitializeAbilities(combatantEntity, combatantAbilityEntities);
                
                foreach (CombatantAbilityEntity combatantAbilityEntity in combatantAbilityEntities)
                { 
                    combatantAbilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = readyTime });
                    
                    TriggerComponent triggerComponent = combatantAbilityEntity.GetComponent<TriggerComponent>();
                    if (triggerComponent.TriggerEventType == TriggerEventType.ABILITY_READY)
                    {
                        _abilityEventScheduler.ScheduleEvent(readyTime, combatantAbilityEntity.AbilityID, abilityStageIndex: 0, combatantAbilityEntity.CombatantID);
                    }
                    else
                    { 
                        _triggerSubscriber.SubscribeAbility(combatantAbilityEntity);
                    }
                }
            }
        }

        private static uint GetCombatantInitiative(CombatantEntity combatantEntity)
        { 
            AgilityComponent agilityComponent = combatantEntity.GetComponent<AgilityComponent>();

            return agilityComponent.Initiative;
        }
    }   
}