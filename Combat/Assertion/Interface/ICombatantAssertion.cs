using IdelPog.Combat.Combatant.Runtime.Entity;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICombatantAssertion
    {
        public void AssertCombatantAlive(CombatantEntity combatantEntity);
    }
}