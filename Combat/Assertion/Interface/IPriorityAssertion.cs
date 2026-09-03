using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface IPriorityAssertion
    {
        public void AssertPriority(ImmutableArray<AbilityStageCard> abilityStageCards, IReadOnlyList<StrategyCard> strategyCard);
    }
}