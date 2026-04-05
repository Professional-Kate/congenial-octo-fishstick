using IdelPog.Combat.Contracts;
using IdelPog.Combat.Runtime;

namespace IdelPog.Combat.Service.Logging.Interface
{
    public interface ICombatantLogger
    {
        public void LogCombatantChange(CombatantEntity changedEntity);
        
        public IReadOnlyList<CombatantStateChange> GetStateChanges();
        
        public void ClearStateChanges();
    }
}