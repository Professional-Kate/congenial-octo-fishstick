using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal sealed class CombatantTracker
    {
        internal CombatantCreation CombatantCreation { get; set; }
        internal int TotalAttacks { get; set; }
        
        internal CombatantTracker(int totalAttacks)
        {
            TotalAttacks = totalAttacks;
        }
        
        internal CombatantTracker(CombatantCreation combatantCreation)
        {
            CombatantCreation = combatantCreation;
        }
    }
}