using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Runtime.Event.Resolver
{
    public sealed class HealingAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityHealingService _entityHealingService;

        public HealingAbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityHealingService entityHealingService) 
            : base(combatantRepository, targetFinder, combatantLogger)
        {
            _entityHealingService = entityHealingService;
        }

        protected private override IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity, CombatantAbilityStage combatantAbilityStage)
        {
            IReadOnlyList<CombatantEntity> targetCombatants = GetTargetCombatants(combatantAbilityStage, combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType);
            _entityHealingService.ApplyHealing(targetCombatants, combatantEntity, combatantAbilityStage, tick);

            return targetCombatants;
        }
    }
}