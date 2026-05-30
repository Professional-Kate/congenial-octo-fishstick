using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Logging.Interface
{
    public interface ICombatantLogger
    {
        public void LogCombatantChange(CombatantEntity changedEntity, byte attackerID, AbilityType abilityType, uint damageDealt, double tick);
        
        public IReadOnlyList<CombatantStateChange> GetStateChanges();
        
        public void ClearStateChanges();
    }
}