using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Logging.Contracts;

namespace IdelPog.Combat.Core.Logging.Interface
{
    public interface ICombatantLogger
    {
        public void LogCombatantChange(double tick, CombatantEntity initiatingCombatant, IReadOnlyList<CombatantEntity> targetCombatants, AbilityStage abilityStage, byte abilityID);
        
        public CombatStage[] GetStateChanges();
    }
}