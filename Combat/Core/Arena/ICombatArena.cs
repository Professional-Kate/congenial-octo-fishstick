using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Logging.Contracts;

namespace IdelPog.Combat.Core.Arena
{
    public interface ICombatArena
    { 
        public CombatArenaLog RunCombatSimulation(IReadOnlyList<CombatantDefinition> friendlyCombatantDefinitions, IReadOnlyList<CombatantDefinition> enemyCombatantDefinitions);
    }
}