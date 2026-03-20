using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantStore : ICombatantStore
    {
        public LowestHealthCombatant LowestHealthCombatant { get; private set; }
        public HighestAttackCombatant HighestAttackCombatant { get; private set; }
        
        public void RegisterCombatantChange(byte combatantID, StatCard statCard)
        {
            if (statCard.Health < LowestHealthCombatant.Health)
            { 
                LowestHealthCombatant = new LowestHealthCombatant { Health = statCard.Health, CombatantID = combatantID };
            } 
            else if (statCard.Attack > HighestAttackCombatant.Attack)
            {
                HighestAttackCombatant = new HighestAttackCombatant { Attack = statCard.Attack,  CombatantID = combatantID };
            }
        }

        public void RegisterCombatantDeath(byte combatantID)
        {
            if (combatantID == LowestHealthCombatant.CombatantID)
            {
                // TODO: find lowest health combatant from ECS
            }
            else if (combatantID == HighestAttackCombatant.CombatantID)
            {
                // TODO: find highest attack combatant from ECS
            }
        }
    }
}