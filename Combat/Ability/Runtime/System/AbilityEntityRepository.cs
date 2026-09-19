using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;

namespace IdelPog.Combat.Ability.Runtime.System
{
    public sealed class AbilityEntityRepository : IAbilityEntityRepository
    {
        private AbilityEntity[] _abilityEntities = [];
        
        public void SeedAbilities(AbilityEntity[] combatantAbilities) => _abilityEntities = combatantAbilities;

        public bool Contains(byte combatantID)
        {
            foreach (AbilityEntity combatantAbilityEntity in _abilityEntities)
            {
                if (combatantAbilityEntity.InstanceID == combatantID)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<AbilityEntity> EnumerateAbilities(byte instanceID)
        {
            foreach (AbilityEntity combatantAbilityEntity in _abilityEntities)
            {
                if (combatantAbilityEntity.InstanceID != instanceID)
                {
                    continue;
                }

                yield return combatantAbilityEntity;
            }
        }

        public void Clear() => _abilityEntities = [];
    }
}