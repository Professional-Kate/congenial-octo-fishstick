using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Store.Interface
{
    public interface ICombatantStoreService
    {
        public void RegisterInitialTargets();
        
        public void RegisterCombatantChange(CombatantEntity combatantEntity);
        
        public void RegisterCombatantDeath(CombatantEntity deadCombatant);
    }
}