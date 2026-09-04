using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Card;

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
            AssertStatCard(combatantCreation.HealthCard);
            AssertAgilityCard(combatantCreation.AgilityCard);
        }

        private void AssertStatCard(HealthCard healthCard)
        {
            _numberAssertion.AssertNumberNotZero(healthCard.Health, nameof(healthCard.Health));
        }

        private void AssertAgilityCard(AgilityCard agilityCard)
        {
            _numberAssertion.AssertNumberNotZero(agilityCard.Speed, nameof(agilityCard.Speed));
            _numberAssertion.AssertNumberNotZero(agilityCard.Initiative, nameof(agilityCard.Initiative));
        }
    }
}