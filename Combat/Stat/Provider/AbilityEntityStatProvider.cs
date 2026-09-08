using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Provider.Interface;

namespace IdelPog.Combat.Stat.Provider
{
    public sealed class AbilityEntityStatProvider : IStatProvider
    {
        private readonly IAbilityEntityRepository _abilityEntityRepository;

        public AbilityEntityStatProvider(IAbilityEntityRepository abilityEntityRepository)
        {
            _abilityEntityRepository = abilityEntityRepository;
        }

        public uint GetStat(CombatantEntity combatantEntity, StatType statType)
        {
            // TODO: enum for the strategy to calculate the stat 
            uint abilityStat = 0;
            foreach (AbilityEntity abilityEntity in _abilityEntityRepository.EnumerateAbilities(combatantEntity.InstanceID))
            {
                abilityStat += abilityEntity.GetStat(statType);
            }
            
            return abilityStat;
        }
    }
}