using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
{
    public sealed class SpeedProvider : IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity)
        {
            AgilityComponent statsComponent = combatantEntity.GetComponent<AgilityComponent>();
            return statsComponent.Speed;
        }
    }
}