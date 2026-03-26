using IdelPog.Combat.Runtime.Component;

namespace IdelPog.Combat.Runtime.System.Store.Interface
{
    public interface ICombatantStore : ICombatantStoreRead
    {
        public void RegisterInitial(IEnumerable<CombatantEntity> combatants);
        
        public void RegisterCombatantChange(byte combatantID, CombatantStatsComponent combatantStatsComponent);

        public void RegisterCombatantDeath(byte combatantID, IEnumerable<CombatantEntity> combatants);
    }
}