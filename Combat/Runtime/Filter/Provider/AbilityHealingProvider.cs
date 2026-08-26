using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
{
    public sealed class AbilityHealingProvider : IStatProvider
    {
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;

        public AbilityHealingProvider(ICombatantAbilityEntityRepository combatantAbilityEntityRepository)
        {
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
        }

        public uint GetStat(CombatantEntity combatantEntity)
        {
            uint healing = 0;
            foreach (CombatantAbilityEntity combatantAbilityEntity in _combatantAbilityEntityRepository.GetAll(combatantEntity.CombatantID))
            {
                AbilityHealingComponent abilityHealingComponent = combatantAbilityEntity.GetComponent<AbilityHealingComponent>();
                healing += abilityHealingComponent.TotalHealing;
            }

            return healing;
        }
    }
}