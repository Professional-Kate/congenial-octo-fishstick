using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
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