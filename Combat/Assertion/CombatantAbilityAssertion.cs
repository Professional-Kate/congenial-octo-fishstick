using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Assertion
{
    public sealed class CombatantAbilityAssertion : ICombatantAbilityAssertion
    {
        public required byte MaxAbilities { get; init; }

        public void AssertAbilityCount(CombatantAbilityEquip combatantAbilityEquip)
        {
            if (combatantAbilityEquip.AbilityCards.Length > MaxAbilities)
            {
                throw new TooManyAbilitiesException(combatantAbilityEquip, MaxAbilities);
            }
        }
    }
}