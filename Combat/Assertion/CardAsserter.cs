using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Assertion
{
    public sealed class CardAsserter : ICardAsserter
    {
        private readonly INumberAssertion _numberAssertion;

        public CardAsserter(INumberAssertion numberAssertion)
        {
            _numberAssertion = numberAssertion;
        }

        // TODO: test aaaaaa
        public void AssertCombatantCards(CombatantCreation combatantCreation)
        {
            AssertStatCard(combatantCreation.StatCard);
            AssertAgilityCard(combatantCreation.AgilityCard);
        }

        public void AssertStatCard(StatCard statCard)
        {
            _numberAssertion.AssertNumberNotZero(statCard.Health, nameof(statCard.Health));
        }

        public void AssertAgilityCard(AgilityCard agilityCard)
        {
            _numberAssertion.AssertNumberNotZero(agilityCard.Speed, nameof(agilityCard.Speed));
            _numberAssertion.AssertNumberNotZero(agilityCard.Initiative, nameof(agilityCard.Initiative));
        }
    }
}