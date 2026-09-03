using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    public interface ICombatantRepository
    {
        public void SeedFriendlyCombatants(CombatantEntity[] friendlyCombatants);
        
        public void SeedEnemyCombatants(CombatantEntity[] enemyCombatants);

        public CombatantEntity Get(byte id);
        
        public IEnumerable<CombatantEntity> Enumerate();

        public void Clear();
    }
}