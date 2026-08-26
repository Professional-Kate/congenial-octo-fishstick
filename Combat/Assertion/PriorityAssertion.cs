using System.Collections.Immutable;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component.Ability;

namespace IdelPog.Combat.Assertion
{
    public sealed class PriorityAssertion : IPriorityAssertion
    {
        public void AssertPriority(ImmutableArray<AbilityStage> abilityStages, IReadOnlyList<StrategyCard> strategyCards)
        {
            if (abilityStages.Length != strategyCards.Count)
            {
                throw new PriorityMissingException(abilityStages.Length, strategyCards.Count);
            }

            for (int i = 0; i < abilityStages.Length; i++)
            {
                AbilityStage abilityStage = abilityStages[i];
                StrategyCard strategyCard = strategyCards[i];
                
                if (abilityStage.Priority != strategyCard.Priority)
                { 
                    throw new PriorityMismatchException(abilityStage.Priority, strategyCard.Priority);
                }
            }
        }
    }
}