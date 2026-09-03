using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Filter.Provider.Interface;

namespace IdelPog.Combat.Core.Filter.Provider
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