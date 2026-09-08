using IdelPog.Combat.Ability.Contracts;

namespace IdelPog.Combat.Ability.Service.Interface
{
    public interface IAbilitySlotCalculator
    {
        public uint GetAbilitySlots(EquippedAbility[] abilityStages);
    }
}