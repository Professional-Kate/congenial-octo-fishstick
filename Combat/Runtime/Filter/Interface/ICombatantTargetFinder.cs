using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Runtime.Filter.Interface
{
    public interface ICombatantTargetFinder
    {
        public IEnumerable<CombatantEntity> SelectPreferredTargets(TargetingPreference targetingPreference, CombatantStatType combatantStatType, TargetingType targetingType, TargetingType casterTargetingType, byte targetCount);
    }
}