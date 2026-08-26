using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
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

        public void ResolveEffect(double tick, CombatantAbilityEntity combatantAbilityEntity, CombatantAbilityStage combatantAbilityStage)
        {
            CombatantEntity combatantEntity = _combatantRepository.Get(combatantAbilityEntity.CombatantID);
            if (combatantEntity.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                // the Combatant could die before this ability stage can resolve
                return;
            }

            if (CanResolve(combatantEntity, combatantAbilityEntity) == false)
            {
                return;
            }
            
            BeforeEvent(tick, combatantEntity, combatantAbilityStage);

            IReadOnlyList<CombatantEntity> changedTargets = HandleEvent(tick, combatantEntity, combatantAbilityEntity, combatantAbilityStage);
            _combatantLogger.LogCombatantChange(tick, combatantEntity, changedTargets, combatantAbilityStage.AbilityStage, combatantAbilityEntity.AbilityID);
            
            AfterEvent(tick, changedTargets, combatantAbilityStage);
        }

        protected private IReadOnlyList<CombatantEntity> GetTargetCombatants(CombatantAbilityStage combatantAbilityStage, TargetingType castersTargetingType)
        {
            TargetingPreferenceComponent targetingPreferenceComponent = combatantAbilityStage.TargetingPreferenceComponent;
            
            return _targetFinder.SelectPreferredTargets(targetingPreferenceComponent.TargetingPreference, targetingPreferenceComponent.CombatantStatType, targetingPreferenceComponent.TargetingType, castersTargetingType, combatantAbilityStage.AbilityStage.MaxTargets).ToArray();
        }
        
        protected private CombatantEntity GetCombatant(byte combatantID) => _combatantRepository.Get(combatantID);

        protected private virtual bool CanResolve(CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity) => true;

        protected private virtual void BeforeEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityStage combatantAbilityStage) { }
        
        protected private abstract IReadOnlyList<CombatantEntity> HandleEvent(double tick, CombatantEntity combatantEntity, CombatantAbilityEntity combatantAbilityEntity, CombatantAbilityStage combatantAbilityStage);

        protected private virtual void AfterEvent(double tick, IEnumerable<CombatantEntity> combatantEntities, CombatantAbilityStage combatantAbilityStage) { }
    }
}