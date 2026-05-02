using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Service
{
    public sealed class AbilitySlotCalculator : IAbilitySlotCalculator
    {
        private readonly IAssetRepository<AbilityType, AbilityEntity> _skillEntityRepository;

        public AbilitySlotCalculator(IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository)
        {
            _skillEntityRepository = skillEntityRepository;
        }
        
        public byte GetAbilitySlots(AbilityCard[] abilityCards)
        {
            byte reservedAbilitySlots = 0;
            foreach (AbilityCard abilityCard in abilityCards)
            {
                AbilityEntity abilityEntity = _skillEntityRepository.Get(abilityCard.AbilityType);
                reservedAbilitySlots += abilityEntity.AbilitySlots;
            }

            return reservedAbilitySlots;
        }
    }
}