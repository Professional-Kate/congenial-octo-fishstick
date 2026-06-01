using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
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