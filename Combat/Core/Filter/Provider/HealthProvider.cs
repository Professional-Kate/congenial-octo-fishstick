using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Filter.Provider.Interface;

namespace IdelPog.Combat.Core.Filter.Provider
{
    public sealed class HealthProvider : IStatProvider
    {
        public uint GetStat(CombatantEntity combatantEntity)
        {
            HealthComponent healthComponent = combatantEntity.GetComponent<HealthComponent>();
            return healthComponent.Health;
        }
    }
} 