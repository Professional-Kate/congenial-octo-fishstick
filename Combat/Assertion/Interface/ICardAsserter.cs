using IdelPog.Combat.Combatant.Contracts.Command;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICardAsserter
    {
        public void AssertCombatantCards(CombatantCreation combatantCreation);
    }
}