using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Filter.Interface
{
    public interface ICombatantTargetFinder
    {
        public IEnumerable<CombatantEntity> SelectPreferredTargets(TargetingPreference targetingPreference, StatType statType, TargetingType targetingType, TargetingType casterTargetingType, byte targetCount);
    }
}