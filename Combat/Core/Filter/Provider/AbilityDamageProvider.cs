using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Filter.Provider.Interface;

namespace IdelPog.Combat.Core.Filter.Provider
{
    public sealed class AbilityDamageProvider : IStatProvider
    {
        private readonly IAbilityEntityRepository _abilityEntityRepository;

        public AbilityDamageProvider(IAbilityEntityRepository abilityEntityRepository)
        {
            _abilityEntityRepository = abilityEntityRepository;
        }

        public uint GetStat(CombatantEntity combatantEntity)
        {
            uint damage = 0;
            foreach (AbilityEntity combatantAbilityEntity in _abilityEntityRepository.EnumerateAbilities(combatantEntity.InstanceID))
            {
                AbilityDamageComponent abilityDamageComponent = combatantAbilityEntity.GetComponent<AbilityDamageComponent>();
                damage += abilityDamageComponent.TotalDamage;
            }

            return damage;
        }
    }
}