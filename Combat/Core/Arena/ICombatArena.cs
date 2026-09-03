using IdelPog.Combat.Combatant.Model;

namespace IdelPog.Combat.Core.Arena
{
    public interface ICombatArena
    { 
        public void RunCombatSimulation(IReadOnlyList<CombatantDefinition> friendlyCombatantDefinitions, IReadOnlyList<CombatantDefinition> enemyCombatantDefinitions);
    }
}