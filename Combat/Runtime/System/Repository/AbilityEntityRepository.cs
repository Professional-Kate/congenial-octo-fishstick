using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Runtime.System.Repository.Interface;

namespace IdelPog.Combat.Runtime.System.Repository
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

        public AbilityEntity Get(byte instanceID, byte abilityID)
        { 
            foreach (AbilityEntity combatantAbilityEntity in EnumerateAbilities(instanceID))
            { 
                if (combatantAbilityEntity.AbilityID == abilityID)
                {
                    return combatantAbilityEntity;
                }
            }
            
            throw new KeyNotFoundException();
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