using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Interface
{
    public interface IAbilitySlotCalculator
    {
        public byte GetAbilitySlots(CombatantAbilityCard[] abilityCards, IReadOnlyList<CombatantAbilityEntity> existingEntities);
    }
}