using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICombatantAssertion
    {
        public void AssertCombatantAlive(CombatantEntity combatantEntity);
    }
}