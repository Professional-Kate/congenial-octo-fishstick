using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ICombatantAbilityAssertion
    {
        public void AssertAbilityCount(CombatantAbilityEquip combatantAbilityEquip);
    }
}