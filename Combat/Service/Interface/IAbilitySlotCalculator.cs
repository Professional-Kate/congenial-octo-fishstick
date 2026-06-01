using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Service.Interface
{
    public interface IAbilitySlotCalculator
    {
        public byte GetAbilitySlots(CombatantAbilityCard[] abilityCards);
    }
}