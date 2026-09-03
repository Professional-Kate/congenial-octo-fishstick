using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class InitialAbilityScheduler : IInitialAbilityScheduler
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly IAbilityEntityRepository _abilityEntityRepository;
        private readonly IAbilityInitializer _abilityInitializer;
        private readonly IAbilityEventScheduler _abilityEventScheduler;
        private readonly ITriggerSubscriber _triggerSubscriber;

        public InitialAbilityScheduler(ICombatantRepository combatantRepository, IAbilityEntityRepository abilityEntityRepository, IAbilityInitializer abilityInitializer, IAbilityEventScheduler abilityEventScheduler, ITriggerSubscriber triggerSubscriber)
        {
            _combatantRepository = combatantRepository;
            _abilityEntityRepository = abilityEntityRepository;
            _abilityInitializer = abilityInitializer;
            _abilityEventScheduler = abilityEventScheduler;
            _triggerSubscriber = triggerSubscriber;
        }

        public void ScheduleRegisteredAbilities(double initialTick)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.Enumerate())
            {
                // if no Abilities have been created for CombatantID, then we have nothing to enqueue
                if (_abilityEntityRepository.Contains(combatantEntity.InstanceID) == false)
                {
                    continue;
                } 
                
                double readyTime = initialTick - GetCombatantInitiative(combatantEntity) * 0.05;
                
                IReadOnlyList<AbilityEntity> abilityEntities = _abilityEntityRepository.EnumerateAbilities(combatantEntity.InstanceID).ToArray();
                _abilityInitializer.InitializeAbilities(combatantEntity, abilityEntities);
                
                foreach (AbilityEntity abilityEntity in abilityEntities)
                {
                    abilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = readyTime });
                    
                    TriggerComponent triggerComponent = abilityEntity.GetComponent<TriggerComponent>();
                    if (triggerComponent.TriggerEventType == TriggerEventType.ABILITY_READY)
                    {
                        _abilityEventScheduler.ScheduleEvent(readyTime, abilityEntity.AbilityID, abilityStageIndex: 0, abilityEntity.InstanceID);
                    }
                    else
                    { 
                        _triggerSubscriber.SubscribeAbility(abilityEntity);
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