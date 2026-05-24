using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICardAsserter
    {
        public void AssertCombatantCards(CombatantCreation combatantCreation);
        
        public void AssertStatCard(StatCard statCard);
        
        public void AssertAgilityCard(AgilityCard agilityCard);
    }
}