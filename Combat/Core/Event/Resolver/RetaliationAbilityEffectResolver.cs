using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Filter.Interface;
using IdelPog.Combat.Core.Logging;

namespace IdelPog.Combat.Core.Event.Resolver
{
    public sealed class RetaliationAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityDamageSystem _entityDamageSystem;
        
        public RetaliationAbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityDamageSystem entityDamageSystem) 
            : base(combatantRepository, targetFinder, combatantLogger)
        {
            _entityDamageSystem = entityDamageSystem;
        }

        protected private override bool CanResolve(CombatantEntity combatantEntity, AbilityEntity abilityEntity)
        {
                Console.WriteLine(combatantEntity.InstanceID);
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
            for (int i = 0; i < abilityStage.AbilityStageCards.MaxTargets; i++)
            {
                if (retaliationComponent.TryDequeue(out CombatantDamageComponent combatantDamageComponent) == false)
                {
                    break;
                }
                

                if (targetCombatantIDs.Add(combatantDamageComponent.CombatantID) == false)
                {
                    continue;
                }
                
                targetCombatants.Add(GetCombatant(combatantDamageComponent.CombatantID));
            }
            
            _entityDamageSystem.ApplyDamage(targetCombatants, combatantEntity.InstanceID, abilityStage, tick);

            return targetCombatants;
        }
    }
}