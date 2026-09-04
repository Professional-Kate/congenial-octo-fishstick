using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Stat.Provider.Interface;

namespace IdelPog.Combat.Stat.Provider
{
    public sealed class BaseHealthProvider : IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity)
        {
            BaseHealthComponent baseHealthComponent = combatantEntity.GetComponent<BaseHealthComponent>();
            return baseHealthComponent.Health;
        }
    }
}