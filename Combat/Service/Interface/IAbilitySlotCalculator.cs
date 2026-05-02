using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Service.Interface
{
    public interface IAbilitySlotCalculator
    {
        public byte GetAbilitySlots(AbilityCard[] abilityCards);
    }
}