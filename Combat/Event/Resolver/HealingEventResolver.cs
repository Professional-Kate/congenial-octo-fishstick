using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class HealingEventResolver : EventResolver
    {
        private readonly IEntityHealingService _entityHealingService;

        public HealingEventResolver(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatantTargetFinder targetFinder, IAbilityEventScheduler abilityEventScheduler, IEntityHealingService entityHealingService) 
            : base(combatantRepository, combatantAbilityEntityRepository, targetFinder, abilityEventScheduler)
        {
            _entityHealingService = entityHealingService;
        }

        protected private override void HandleEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity)
        {
            FriendlyStatusComponent friendlyStatusComponent = combatantEntity.GetComponent<FriendlyStatusComponent>();

            IEnumerable<CombatantEntity> targetCombatants = GetTargetCombatants(combatantAbilityEntity, friendlyStatusComponent.IsFriendly);
            _entityHealingService.ApplyHealing(targetCombatants, combatantEntity, combatantAbilityEntity, tick);
        }
    }
}