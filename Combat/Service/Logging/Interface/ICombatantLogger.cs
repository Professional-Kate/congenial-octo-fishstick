using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Logging.Interface
{
    public interface ICombatantLogger
    {
        public void LogCombatantChange(CombatantEntity changedEntity, byte attackerID, AbilityType abilityType, uint damageDealt);
        
        public IReadOnlyList<CombatantStateChange> GetStateChanges();
        
        public void ClearStateChanges();
    }
}