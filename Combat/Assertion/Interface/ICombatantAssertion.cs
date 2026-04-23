using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICombatantAssertion
    {
        public void AssertCombatantAlive(CombatantEntity combatantEntity);
    }
}