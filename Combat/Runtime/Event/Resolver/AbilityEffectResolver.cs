using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Runtime.Event.Resolver
{
    public abstract class AbilityEffectResolver : IAbilityEffectResolver
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICombatantTargetFinder _targetFinder;
        private readonly ICombatantLogger _combatantLogger;

        protected private AbilityEffectResolver(ICombatantRepository combatantRepository, ICombatantTargetFinder targetFinder, ICombatantLogger combatantLogger)
        {
            _combatantRepository = combatantRepository;
            _targetFinder = targetFinder;
            _combatantLogger = combatantLogger;
        }

        public void ResolveEffect(double tick, AbilityEntity abilityEntity, AbilityStage abilityStage)
        {
            CombatantEntity combatantEntity = _combatantRepository.Get(abilityEntity.InstanceID);
            if (combatantEntity.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                // the Combatant could die before this ability stage can resolve
                return;
            }

            if (CanResolve(combatantEntity, abilityEntity) == false)
            {
                return;
            }
            
            BeforeEvent(tick, combatantEntity, abilityStage);

            IReadOnlyList<CombatantEntity> changedTargets = HandleEvent(tick, combatantEntity, abilityEntity, abilityStage);
            _combatantLogger.LogCombatantChange(tick, combatantEntity, changedTargets, abilityStage.AbilityStageCards, abilityEntity.AbilityID);
            
            AfterEvent(tick, changedTargets, abilityStage);
        }

        protected private IReadOnlyList<CombatantEntity> GetTargetCombatants(AbilityStage abilityStage, TargetingType targetingType)
        {
            TargetingPreferenceComponent targetingPreferenceComponent = abilityStage.TargetingPreferenceComponent;
            
            return _targetFinder.SelectPreferredTargets(targetingPreferenceComponent.TargetingPreference, targetingPreferenceComponent.CombatantStatType, targetingPreferenceComponent.TargetingType, targetingType, abilityStage.AbilityStageCards.MaxTargets).ToArray();
        }
        
        protected private CombatantEntity GetCombatant(byte combatantID) => _combatantRepository.Get(combatantID);

        protected private virtual bool CanResolve(CombatantEntity combatantEntity, AbilityEntity abilityEntity) => true;

        protected private virtual void BeforeEvent(double tick, CombatantEntity combatantEntity, AbilityStage abilityStage) { }
        
        protected private abstract IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, AbilityEntity abilityEntity, AbilityStage abilityStage);

        protected private virtual void AfterEvent(double tick, IEnumerable<CombatantEntity> combatantEntities, AbilityStage abilityStage) { }
    }
}