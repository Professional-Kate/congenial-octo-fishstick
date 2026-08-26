using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Incremental;

namespace IdelPog.Combat.Service
{
    public sealed class AbilitySlotCalculator : IAbilitySlotCalculator
    {
        private readonly IIncrementalRepository<AbilityEntity> _abilityEntityRepository;

        public AbilitySlotCalculator(IIncrementalRepository<AbilityEntity> abilityEntityRepository)
        {
            _abilityEntityRepository = abilityEntityRepository;
        }

        public byte GetAbilitySlots(CombatantAbilityCard[] abilityCards, IReadOnlyList<CombatantAbilityEntity> existingEntities)
        {
            byte reservedAbilitySlots = 0;
            foreach (CombatantAbilityCard abilityCard in abilityCards)
            {
                AbilityEntity abilityEntity = _abilityEntityRepository.Get(abilityCard.AbilityID);
                reservedAbilitySlots += abilityEntity.AbilitySlots;
            }
            
            foreach (CombatantAbilityEntity combatantAbilityEntity in existingEntities)
            {
                reservedAbilitySlots += combatantAbilityEntity.AbilitySlots;
            }

            return reservedAbilitySlots;
        }
    }
}