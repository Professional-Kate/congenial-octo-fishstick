using IdelPog.Combat.Combatant.Contracts;

namespace IdelPog.Combat.Service.Interface
{
    public interface IAbilitySlotCalculator
    {
        public byte GetAbilitySlots(EquippedAbility[] abilityStages);
    }
}