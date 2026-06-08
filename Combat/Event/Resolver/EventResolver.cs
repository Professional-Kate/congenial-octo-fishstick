using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public abstract class EventResolver : IEventResolver
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly ICombatantTargetFinder _targetFinder;
        private readonly IAbilityEventScheduler _abilityEventScheduler;

        protected private EventResolver(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatantTargetFinder targetFinder, IAbilityEventScheduler abilityEventScheduler)
        {
            _combatantRepository = combatantRepository;
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _targetFinder = targetFinder;
            _abilityEventScheduler = abilityEventScheduler;
        }

        public void ResolveEvent(double tick, byte combatantID, AbilityType abilityType)
        {
            CombatantEntity combatantEntity = _combatantRepository.Get(combatantID);
            if (combatantEntity.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                // the Combatant could die before this Event can resolve
                return;
            }
            
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(combatantID, abilityType);
            
            BeforeEvent();

            HandleEvent(tick, combatantEntity, combatantAbilityEntity);
            
            AfterEvent(tick, combatantAbilityEntity);
        }

        protected private IEnumerable<CombatantEntity> GetTargetCombatants(CombatantAbilityEntity combatantAbilityEntity, bool isFriendly)
        {
            TargetingPreferenceComponent targetingPreferenceComponent = combatantAbilityEntity.GetComponent<TargetingPreferenceComponent>();
            
            return _targetFinder.SelectPreferredTargets(targetingPreferenceComponent.TargetingPreference, targetingPreferenceComponent.CombatantStatType, isFriendly, 1);
        }

        protected private virtual void BeforeEvent()
        {
            // TODO: I have no idea what arguments are needed for this right now. 
            //  soon as I know I'll update this :)
        }
        
        protected private abstract void HandleEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity);

        /// <summary>
        /// Reschedules the <see cref="AbilityType"/> for <paramref name="tick"/> plus <paramref name="combatantAbilityEntity"/> Cooldown
        /// </summary>
        /// <param name="tick">The initial tick the event was resolved at</param>
        /// <param name="combatantAbilityEntity">What <see cref="AbilityType"/> the Entity used</param>
        protected private virtual void AfterEvent(double tick, CombatantAbilityEntity combatantAbilityEntity)
        {
            CooldownComponent cooldownComponent = combatantAbilityEntity.GetComponent<CooldownComponent>();
            _abilityEventScheduler.ScheduleEvent(tick + cooldownComponent.Cooldown, combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType);
        }
    }
}