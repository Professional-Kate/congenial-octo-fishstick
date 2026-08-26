using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Incremental;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class CombatantAbilityEntityFactory : ICombatantAbilityEntityFactory
    {
        private readonly IIncrementalRepository<AbilityEntity> _abilityEntityRepository;
        private readonly IPrioritySorter _prioritySorter;
        private readonly IAbilityEffectValueCalculator _abilityEffectValueCalculator;
        private readonly IPriorityAssertion _priorityAssertion;

        public CombatantAbilityEntityFactory(IIncrementalRepository<AbilityEntity> abilityEntityRepository, IPrioritySorter prioritySorter, IAbilityEffectValueCalculator abilityEffectValueCalculator, IPriorityAssertion priorityAssertion)
        {
            _abilityEntityRepository = abilityEntityRepository;
            _prioritySorter = prioritySorter;
            _abilityEffectValueCalculator = abilityEffectValueCalculator;
            _priorityAssertion = priorityAssertion;
        }

        public IReadOnlyList<CombatantAbilityEntity> Create(CombatantAbilityEquip combatantAbilityEquip)
        {
            CombatantAbilityEntity[] combatantAbilityEntities = new CombatantAbilityEntity[combatantAbilityEquip.AbilityCards.Length];
            for (int i = 0; i < combatantAbilityEquip.AbilityCards.Length; i++)
            {
                CombatantAbilityCard combatantAbilityCard = combatantAbilityEquip.AbilityCards[i];
                AbilityEntity abilityEntity = _abilityEntityRepository.Get(combatantAbilityCard.AbilityID);

                AbilityStagesComponent abilityStagesComponent = new() { AbilityStages = [..ConvertAbilityStages(combatantAbilityCard.StrategyCards, abilityEntity)] };
                CombatantAbilityEntity combatantAbilityEntity = AddBaseComponents(abilityEntity, combatantAbilityEquip.CombatantID, combatantAbilityCard.AbilityID, abilityStagesComponent);
                _abilityEffectValueCalculator.Calculate(combatantAbilityEntity);
                    
                combatantAbilityEntities[i] = combatantAbilityEntity;
            }
            
            return combatantAbilityEntities;
        }

        private CombatantAbilityStage[] ConvertAbilityStages(StrategyCard[] strategyCards, AbilityEntity abilityEntity)
        {
            IReadOnlyList<StrategyCard> sortedStrategyCards = _prioritySorter.Sort(strategyCards, card => card.Priority);
            _priorityAssertion.AssertPriority(abilityEntity.AbilityStages, sortedStrategyCards);
                    
            CombatantAbilityStage[] combatantAbilityStages = new CombatantAbilityStage[abilityEntity.AbilityStages.Length];
            for (int index = 0; index < abilityEntity.AbilityStages.Length; index++)
            {
                combatantAbilityStages[index] = CreateCombatantAbilityStage(abilityEntity.AbilityStages[index], sortedStrategyCards[index]);
            }
            
            return combatantAbilityStages;
        }

        private static CombatantAbilityStage CreateCombatantAbilityStage(AbilityStage abilityStage, StrategyCard strategyCard)
        {
            TargetingPreferenceComponent targetingPreferenceComponent = new()
            {
                CombatantStatType = strategyCard.CombatantStatType, 
                TargetingPreference = strategyCard.TargetingPreference, 
                TargetingType = strategyCard.TargetingType
            };
                
            return new CombatantAbilityStage { AbilityStage = abilityStage, TargetingPreferenceComponent = targetingPreferenceComponent};
        }
        
        private static CombatantAbilityEntity AddBaseComponents(AbilityEntity abilityEntity, byte combatantID, byte abilityID, AbilityStagesComponent abilityStagesComponent)
        {
            CombatantAbilityEntity combatantAbilityEntity = new(abilityEntity.GetComponent<CooldownComponent>(), abilityEntity.GetComponent<TriggerComponent>(), abilityStagesComponent)
            {
                CombatantID = combatantID,
                AbilityID = abilityID,
                AbilitySlots = abilityEntity.AbilitySlots
            };

            return combatantAbilityEntity;
        }
    }
}