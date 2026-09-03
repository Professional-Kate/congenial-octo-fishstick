using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Assertion
{
    public sealed class CardAsserter : ICardAsserter
    {
        private readonly INumberAssertion _numberAssertion;

        public CardAsserter(INumberAssertion numberAssertion)
        {
            _numberAssertion = numberAssertion;
        }

        public void AssertCombatantCards(CombatantCreation combatantCreation)
        {
            AssertStatCard(combatantCreation.StatCard);
            AssertAgilityCard(combatantCreation.AgilityCard);
        }

        private void AssertStatCard(StatCard statCard)
        {
            _numberAssertion.AssertNumberNotZero(statCard.Health, nameof(statCard.Health));
        }

        private void AssertAgilityCard(AgilityCard agilityCard)
        {
            _numberAssertion.AssertNumberNotZero(agilityCard.Speed, nameof(agilityCard.Speed));
            _numberAssertion.AssertNumberNotZero(agilityCard.Initiative, nameof(agilityCard.Initiative));
        }
    }
}