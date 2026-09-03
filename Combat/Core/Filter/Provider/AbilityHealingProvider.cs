using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Filter.Provider.Interface;

namespace IdelPog.Combat.Core.Filter.Provider
{
    public sealed class AbilityHealingProvider : IStatProvider
    {
        private readonly IAbilityEntityRepository _abilityEntityRepository;

        public AbilityHealingProvider(IAbilityEntityRepository abilityEntityRepository)
        {
            _abilityEntityRepository = abilityEntityRepository;
        }

        public uint GetStat(CombatantEntity combatantEntity)
        {
            uint healing = 0;
            foreach (AbilityEntity abilityEntity in _abilityEntityRepository.EnumerateAbilities(combatantEntity.InstanceID))
            {
                AbilityHealingComponent abilityHealingComponent = abilityEntity.GetComponent<AbilityHealingComponent>();
                healing += abilityHealingComponent.TotalHealing;
            }

            return healing;
        }
    }
}