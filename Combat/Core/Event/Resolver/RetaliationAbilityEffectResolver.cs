using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Logging.Interface;
using IdelPog.Combat.Stat.Filter.Interface;

namespace IdelPog.Combat.Core.Event.Resolver
{
    public sealed class RetaliationAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityDamageSystem _entityDamageSystem;
        
        public RetaliationAbilityEffectResolver(ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityDamageSystem entityDamageSystem) 
            : base(targetFinder, combatantLogger)
        {
            _entityDamageSystem = entityDamageSystem;
        }

        protected private override bool CanResolve(CombatantEntity combatantEntity, AbilityEntity abilityEntity)
        {
            if (combatantEntity.TryGetComponent(out RetaliationComponent retaliationComponent) == false)
            {
                return false;
            }

            return retaliationComponent.Count != 0;
        }

        protected private override IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, AbilityEntity abilityEntity, AbilityStage abilityStage)
        {
            RetaliationComponent retaliationComponent = combatantEntity.GetComponent<RetaliationComponent>();

            HashSet<byte> targetCombatantIDs = [];
            List<CombatantEntity> targetCombatants = [];
            for (int i = 0; i < abilityStage.AbilityStageCard.MaxTargets; i++)
            {
                if (retaliationComponent.TryDequeue(out CombatantDamaged combatantDamageComponent) == false)
                {
                    break;
                }

                if (targetCombatantIDs.Add(combatantDamageComponent.InitiatingCombatant.InstanceID) == false)
                {
                    continue;
                }
                
                targetCombatants.Add(combatantDamageComponent.InitiatingCombatant);
            }
            
            _entityDamageSystem.ApplyDamage(targetCombatants, combatantEntity, abilityStage, tick);

            return targetCombatants;
        }
    }
}