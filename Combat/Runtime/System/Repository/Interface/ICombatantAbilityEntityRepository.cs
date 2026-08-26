using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    public interface ICombatantAbilityEntityRepository
    {
        public void AddAbilities(byte combatantID, IReadOnlyList<CombatantAbilityEntity> combatantAbilities);
        
        public bool Contains(byte combatantID);
        
        public CombatantAbilityEntity Get(byte combatantID, byte abilityID);
        
        public IReadOnlyList<CombatantAbilityEntity> GetAll(byte combatantID);
    }
}