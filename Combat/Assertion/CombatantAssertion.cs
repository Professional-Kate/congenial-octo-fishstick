using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Assertion
{
    public sealed class CombatantAssertion : ICombatantAssertion
    {
        public void AssertCombatantAlive(CombatantEntity combatantEntity)
        {
            if (combatantEntity.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                throw new CombatantDeadException(combatantEntity.CombatantID);
            }
        }
    }
}