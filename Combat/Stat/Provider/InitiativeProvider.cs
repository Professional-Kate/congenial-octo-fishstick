using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Stat.Provider.Interface;

namespace IdelPog.Combat.Stat.Provider
{
    public sealed class InitiativeProvider : IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity)
        { 
            AgilityComponent statsComponent = combatantEntity.GetComponent<AgilityComponent>();
            return statsComponent.Initiative;
        }
    }
}