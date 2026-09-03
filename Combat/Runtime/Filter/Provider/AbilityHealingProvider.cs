using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
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