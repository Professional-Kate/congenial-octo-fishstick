using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component.Ability;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface IPriorityAssertion
    {
        public void AssertPriority(ImmutableArray<AbilityStage> abilityStages, IReadOnlyList<StrategyCard> strategyCard);
    }
}