using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Assertion
{
    public sealed class StatCardAsserter : IStatCardAsserter
    {
        private readonly INumberAssertion _numberAssertion;

        public StatCardAsserter(INumberAssertion numberAssertion)
        {
            _numberAssertion = numberAssertion;
        }

        public void AssertStatCard(StatCard statCard)
        {
            _numberAssertion.AssertNumberNotZero(statCard.Speed, nameof(statCard.Speed));
            _numberAssertion.AssertNumberNotZero(statCard.Health, nameof(statCard.Health));
        }
    }
}