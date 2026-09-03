using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Assertion
{
    public sealed class CombatantAssertion : ICombatantAssertion
    {
        public void AssertCombatantAlive(CombatantEntity combatantEntity)
        {
            if (combatantEntity.GetComponent<LifeStatusComponent>().IsAlive == false)
            {
                throw new CombatantDeadException(combatantEntity.InstanceID);
            }
        }
    }
}