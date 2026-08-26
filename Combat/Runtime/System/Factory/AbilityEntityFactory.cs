using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class AbilityEntityFactory : IAbilityEntityFactory
    {
        private readonly IPrioritySorter _prioritySorter;

        public AbilityEntityFactory(IPrioritySorter prioritySorter)
        {
            _prioritySorter = prioritySorter;
        }

        public AbilityEntity CreateAbilityEntity(AbilityCreation abilityCreation)
        {
            CooldownComponent cooldownComponent = new() { Cooldown = abilityCreation.AbilityCard.Cooldown };
            
            AbilityEntity abilityEntity = new(cooldownComponent, ConvertTriggerCard(abilityCreation.TriggerCard))
            {
                AbilitySlots = abilityCreation.AbilityCard.AbilitySlots,
                AbilityStages = ConvertAbilityStageCards(_prioritySorter.Sort(abilityCreation.AbilityStageCards, card => card.Priority))
            };

            return abilityEntity;
        }

        private static TriggerComponent ConvertTriggerCard(TriggerCard triggerCard)
        {
            return new TriggerComponent
            {
                TargetingType = triggerCard.TargetingType,
                TriggerEventType = triggerCard.TriggerEventType,
                MinTriggerValue = triggerCard.MinTriggerValue,
                MaxTriggerValue = triggerCard.MaxTriggerValue
            };
        }

        private static ImmutableArray<AbilityStage> ConvertAbilityStageCards(IReadOnlyList<AbilityStageCard> sortedAbilityStageCards)
        {
            AbilityStage[] abilityStages = new AbilityStage[sortedAbilityStageCards.Count];
            for (int i = 0; i < sortedAbilityStageCards.Count; i++)
            {
                AbilityStageCard abilityStageCard = sortedAbilityStageCards[i];
                abilityStages[i] = new AbilityStage
                {
                    AbilityEffectType = abilityStageCard.AbilityEffectType, 
                    AffinityType = abilityStageCard.AffinityType, 
                    CastTime = abilityStageCard.CastTime,
                    Value = abilityStageCard.Value, 
                    MaxTargets = abilityStageCard.MaxTargets,
                    Priority =  abilityStageCard.Priority
                };
            }

            return [..abilityStages];
        }
    }
}