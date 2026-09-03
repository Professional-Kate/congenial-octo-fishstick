using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    public interface IAbilityEntityRepository
    {
        public void SeedAbilities(AbilityEntity[] combatantAbilities);
        
        public bool Contains(byte combatantID);
        
        public AbilityEntity Get(byte instanceID, byte abilityID);
        
        public IEnumerable<AbilityEntity> EnumerateAbilities(byte instanceID);

        public void Clear();
    }
}