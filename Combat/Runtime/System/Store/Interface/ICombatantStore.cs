using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Runtime.System.Store.Interface
{
    public interface ICombatantStore : ICombatantStoreGet
    {
        public void RegisterInitial(IEnumerable<CombatantEntity> combatants);
        
        public void RegisterCombatantChange(byte combatantID, StatCard statCard);

        public void RegisterCombatantDeath(byte combatantID, IEnumerable<CombatantEntity> combatants);
    }
}