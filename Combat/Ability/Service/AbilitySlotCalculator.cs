using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Core.Repository.Incremental;

namespace IdelPog.Combat.Ability.Service
{
    public sealed class AbilitySlotCalculator : IAbilitySlotCalculator
    {
        private readonly IIncrementalRepository<AbilityDefinition> _abilityDefinitionRepository;

        public AbilitySlotCalculator(IIncrementalRepository<AbilityDefinition> abilityDefinitionRepository)
        {
            _abilityDefinitionRepository = abilityDefinitionRepository;
        }

        public byte GetAbilitySlots(EquippedAbility[] abilityStages)
        {
            byte reservedAbilitySlots = 0;
            foreach (EquippedAbility abilityCard in abilityStages)
            {
                AbilityDefinition abilityEntity = _abilityDefinitionRepository.Get(abilityCard.AbilityID);
                reservedAbilitySlots += abilityEntity.AbilityCard.AbilitySlots;
            }
            
            return reservedAbilitySlots;
        }
    }
}