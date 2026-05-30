using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
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