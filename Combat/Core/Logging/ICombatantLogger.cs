using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Core.Logging
{
    public interface ICombatantLogger
    {
        public void LogCombatantChange(double tick, CombatantEntity initiatingCombatant, IReadOnlyList<CombatantEntity> targetCombatants, AbilityStageCard abilityStage, byte abilityID);
        
        public IReadOnlyList<CombatStage> GetStateChanges();
        
        public void ClearStateChanges();
    }
}