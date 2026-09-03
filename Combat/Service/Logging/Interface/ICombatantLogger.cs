using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Service.Logging.Interface
{
    public interface ICombatantLogger
    {
        public void LogCombatantChange(double tick, CombatantEntity initiatingCombatant, IReadOnlyList<CombatantEntity> targetCombatants, AbilityStageCard abilityStage, byte abilityID);
        
        public IReadOnlyList<CombatStage> GetStateChanges();
        
        public void ClearStateChanges();
    }
}