using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Runtime.Event.Resolver
{
    public sealed class RetaliationAbilityEffectResolver : AbilityEffectResolver
    {
        private readonly IEntityDamageService _entityDamageService;
        
        public RetaliationAbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger, IEntityDamageService entityDamageService) 
            : base(combatantRepository, targetFinder, combatantLogger)
        {
            _entityDamageService = entityDamageService;
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
            
            _entityDamageService.ApplyDamage(targetCombatants, combatantEntity.InstanceID, abilityStage, tick);

            return targetCombatants;
        }
    }
}