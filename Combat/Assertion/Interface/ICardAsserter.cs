using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICardAsserter
    {
        public void AssertCombatantCards(CombatantCreation combatantCreation);
    }
}