using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
{
    public sealed class AbilityDamageProvider : IStatProvider
    {
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;

        public AbilityDamageProvider(ICombatantAbilityEntityRepository combatantAbilityEntityRepository)
        {
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
        }

        public uint GetStat(CombatantEntity combatantEntity)
        {
            uint damage = 0;
            foreach (CombatantAbilityEntity combatantAbilityEntity in _combatantAbilityEntityRepository.GetAll(combatantEntity.CombatantID))
            {
                AbilityDamageComponent abilityDamageComponent = combatantAbilityEntity.GetComponent<AbilityDamageComponent>();
                damage += abilityDamageComponent.TotalDamage;
            }

            return damage;
        }
    }
}