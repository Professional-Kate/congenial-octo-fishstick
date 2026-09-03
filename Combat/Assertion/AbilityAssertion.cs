using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Assertion
{
    public sealed class AbilityAssertion : IAbilityAssertion
    {
        public required byte MaxAbilitiesSlots { get; init; }

        public void AssertAbilityCount(byte reservedAbilitySlots)
        {
            if (reservedAbilitySlots > MaxAbilitiesSlots)
            { 
                throw new TooManyAbilitiesException(reservedAbilitySlots, MaxAbilitiesSlots);
            }
        }
    }
}