using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Filter.Provider.Interface;

namespace IdelPog.Combat.Core.Filter.Provider
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