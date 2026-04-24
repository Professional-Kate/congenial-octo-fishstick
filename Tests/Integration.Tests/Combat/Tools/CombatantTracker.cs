using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal sealed class CombatantTracker
    {
        internal CombatantCard CombatantCard { get; set; }
        internal int TotalAttacks { get; set; }
        
        internal CombatantTracker(int totalAttacks)
        {
            TotalAttacks = totalAttacks;
        }
        
        internal CombatantTracker(CombatantCard combatantCard)
        {
            CombatantCard = combatantCard;
        }
    }
}