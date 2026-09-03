using System.Collections.Immutable;
using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Assertion
{
    public sealed class PriorityAssertion : IPriorityAssertion
    {
        public void AssertPriority(ImmutableArray<AbilityStageCard> abilityStageCards, IReadOnlyList<StrategyCard> strategyCards)
        {
            if (abilityStageCards.Length != strategyCards.Count)
            {
                throw new PriorityMissingException(abilityStageCards.Length, strategyCards.Count);
            }

            for (int i = 0; i < abilityStageCards.Length; i++)
            {
                AbilityStageCard abilityStage = abilityStageCards[i];
                StrategyCard strategyCard = strategyCards[i];
                
                if (abilityStage.Priority != strategyCard.Priority)
                { 
                    throw new PriorityMismatchException(abilityStage.Priority, strategyCard.Priority);
                }
            }
        }
    }
}