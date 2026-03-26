using IdelPog.Combat.Contracts;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatLog : ICombatLog
    {
        private readonly List<CombatEventLog> _combatEvents = [];

        public void Append(CombatantEntity defendingCombatant, CombatantEntity attackingCombatant)
        {
            CombatEventLog combatEventLog = new()
            {
                AttackerID = attackingCombatant.CombatantID,
                AttackerStatCard =  attackingCombatant.GetComponent<CombatantStatsComponent>().StatCard,
                DefenderID = defendingCombatant.CombatantID, 
                DefenderStatCard =  defendingCombatant.GetComponent<CombatantStatsComponent>().StatCard
            };
            
            _combatEvents.Add(combatEventLog);
        }

        public ICombatLogReader EndCombat()
        {
            ICombatLogReader combatLogReader = new CombatLogReader(_combatEvents.GetEnumerator());
            return combatLogReader;
        }
    }
}