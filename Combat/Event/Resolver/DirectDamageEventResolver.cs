using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    // TODO: abstract EventResolver. Contains a method for registering itself into the ResolverRepository. Contains event hooks for BeforeEvent/AfterEvent
    public sealed class DirectDamageEventResolver : IEventResolver
    {
        private readonly ICombatantTargetFinder _targetFinder;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly IEntityDamageMediator _entityDamageMediator;
        private readonly ICombatantRepository _combatantRepository;
        private readonly IAbilityEventScheduler _abilityEventScheduler;

        public DirectDamageEventResolver(ICombatantTargetFinder targetFinder, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, IEntityDamageMediator entityDamageMediator, ICombatantRepository combatantRepository, IAbilityEventScheduler abilityEventScheduler)
        {
            _targetFinder = targetFinder;
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _entityDamageMediator = entityDamageMediator;
            _combatantRepository = combatantRepository;
            _abilityEventScheduler = abilityEventScheduler;
        }

        public void ResolveEvent(double tick, byte attackerID, AbilityType abilityType)
        {
            CombatantEntity attackingCombatant = _combatantRepository.Get(attackerID);
            if (attackingCombatant.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                // the Combatant could die before this Event can resolve
                return;
            }           
            
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(attackerID, abilityType);
            TargetingPreferenceComponent targetingPreferenceComponent = combatantAbilityEntity.GetComponent<TargetingPreferenceComponent>();
            FriendlyStatusComponent friendlyStatusComponent = attackingCombatant.GetComponent<FriendlyStatusComponent>();
            
            IEnumerable<CombatantEntity> targetCombatants = _targetFinder.SelectPreferredTargets(targetingPreferenceComponent.TargetingPreference, targetingPreferenceComponent.CombatantStatType, !friendlyStatusComponent.IsFriendly, 1);
            _entityDamageMediator.ApplyDamage(targetCombatants, attackingCombatant, combatantAbilityEntity, tick);
            
            CooldownComponent cooldownComponent = combatantAbilityEntity.GetComponent<CooldownComponent>();
            _abilityEventScheduler.ScheduleEvent(tick + cooldownComponent.Cooldown, attackerID, abilityType);
        }
    }
}