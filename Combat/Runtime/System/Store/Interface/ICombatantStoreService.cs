namespace IdelPog.Combat.Runtime.System.Store.Interface
{
    public interface ICombatantStoreService
    {
        public void RegisterInitial();

        public void RegisterCombatantChange(CombatantEntity combatantEntity);
        
        public void RegisterCombatantDeath(CombatantEntity deadCombatant);
    }
}