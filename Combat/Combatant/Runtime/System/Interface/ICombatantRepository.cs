using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Combatant.Runtime.System.Interface
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