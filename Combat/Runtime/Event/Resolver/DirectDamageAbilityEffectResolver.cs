using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Runtime.Event.Resolver
{
    public sealed class DirectDamageAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityDamageService _entityDamageService;

        public DirectDamageAbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityDamageService entityDamageService) 
            : base(combatantRepository, targetFinder, combatantLogger)
        {
            _entityDamageService = entityDamageService;
        }

        protected private override IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity, CombatantAbilityStage combatantAbilityStage)
        {
            IReadOnlyList<CombatantEntity> targetCombatants = GetTargetCombatants(combatantAbilityStage, combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType);
            _entityDamageService.ApplyDamage(targetCombatants, combatantEntity.CombatantID, combatantAbilityStage, tick);

            return targetCombatants;
        }
    }
}