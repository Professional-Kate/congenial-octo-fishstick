using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Resolver.Interface;
using IdelPog.Combat.Core.Logging.Interface;
using IdelPog.Combat.Stat.Filter.Interface;

namespace IdelPog.Combat.Core.Event.Resolver
{
    public abstract class AbilityEffectResolver : IAbilityEffectResolver
    {
        private readonly ICombatantTargetFinder _targetFinder;
        private readonly ICombatantLogger _combatantLogger;

        protected private AbilityEffectResolver(ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger)
        {
            _targetFinder = targetFinder;
            _combatantLogger = combatantLogger;
        }

        public void ResolveEffect(double tick, AbilityEntity abilityEntity, AbilityStage abilityStage, CombatantEntity initiatingCombatant)
        {
            if (initiatingCombatant.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                // the Combatant could die before this ability stage can resolve
                return;
            }

            if (CanResolve(initiatingCombatant, abilityEntity) == false)
            {
                return;
            }
            
            BeforeEvent(tick, initiatingCombatant, abilityStage);

            IReadOnlyList<CombatantEntity> changedTargets = HandleEvent(tick, initiatingCombatant, abilityEntity, abilityStage);
            _combatantLogger.LogCombatantChange(tick, initiatingCombatant, changedTargets, abilityStage, abilityEntity.AbilityID);
            
            AfterEvent(tick, changedTargets, abilityStage);
        }

        protected private IReadOnlyList<CombatantEntity> GetTargetCombatants(AbilityStage abilityStage, TargetingType targetingType)
        {
            TargetingPreferenceComponent targetingPreferenceComponent = abilityStage.TargetingPreferenceComponent;
            
            return _targetFinder.SelectPreferredTargets(targetingPreferenceComponent.TargetingPreference, targetingPreferenceComponent.StatType, targetingPreferenceComponent.TargetingType, targetingType, abilityStage.AbilityStageCard.MaxTargets).ToArray();
        }

        protected private virtual bool CanResolve(CombatantEntity combatantEntity, AbilityEntity abilityEntity) => true;

        protected private virtual void BeforeEvent(double tick, CombatantEntity combatantEntity, AbilityStage abilityStage) { }
        
        protected private abstract IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, AbilityEntity abilityEntity, AbilityStage abilityStage);

        protected private virtual void AfterEvent(double tick, IEnumerable<CombatantEntity> combatantEntities, AbilityStage abilityStage) { }
    }
}