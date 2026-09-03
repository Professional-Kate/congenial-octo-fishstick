using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Filter.Interface;
using IdelPog.Combat.Core.Logging;

namespace IdelPog.Combat.Core.Event.Resolver
{
    public sealed class HealingAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityHealingSystem _entityHealingSystem;

        public HealingAbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityHealingSystem entityHealingSystem) 
            : base(combatantRepository, targetFinder, combatantLogger)
        {
            _entityHealingSystem = entityHealingSystem;
        }

        protected private override IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, AbilityEntity abilityEntity, AbilityStage abilityStage)
        {
            IReadOnlyList<CombatantEntity> targetCombatants = GetTargetCombatants(abilityStage, combatantEntity.TargetingType);
            _entityHealingSystem.ApplyHealing(targetCombatants, combatantEntity, abilityStage, tick);

            return targetCombatants;
        }
    }
}