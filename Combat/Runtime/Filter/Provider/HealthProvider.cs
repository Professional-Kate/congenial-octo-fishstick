using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;

namespace IdelPog.Combat.Runtime.Filter.Provider
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