using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Filter.Interface;
using IdelPog.Combat.Core.Logging;

namespace IdelPog.Combat.Core.Event.Resolver
{
    public sealed class DirectDamageAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityDamageSystem _entityDamageSystem;

        public DirectDamageAbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityDamageSystem entityDamageSystem) 
            : base(combatantRepository, targetFinder, combatantLogger)
        {
            _entityDamageSystem = entityDamageSystem;
        }

        protected private override IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, AbilityEntity abilityEntity, AbilityStage abilityStage)
        {
            IReadOnlyList<CombatantEntity> targetCombatants = GetTargetCombatants(abilityStage, combatantEntity.TargetingType);
            _entityDamageSystem.ApplyDamage(targetCombatants, combatantEntity.InstanceID, abilityStage, tick);

            return targetCombatants;
        }
    }
}