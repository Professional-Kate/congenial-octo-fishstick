using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class DirectDamageEventResolver : EventResolver
    {
        private readonly IEntityDamageService _entityDamageService;

        public DirectDamageEventResolver(ICombatantRepository combatantRepository, ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatantTargetFinder targetFinder, IAbilityEventScheduler abilityEventScheduler, IEntityDamageService entityDamageService) 
            : base(combatantRepository, combatantAbilityEntityRepository, targetFinder, abilityEventScheduler)
        {
            _entityDamageService = entityDamageService;
        }

        protected private override void HandleEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity)
        {
            FriendlyStatusComponent friendlyStatusComponent = combatantEntity.GetComponent<FriendlyStatusComponent>();

            IEnumerable<CombatantEntity> targetCombatants = GetTargetCombatants(combatantAbilityEntity, !friendlyStatusComponent.IsFriendly);
            _entityDamageService.ApplyDamage(targetCombatants, combatantEntity, combatantAbilityEntity, tick);
        }
    }
}