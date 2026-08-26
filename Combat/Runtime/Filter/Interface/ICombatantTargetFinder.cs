using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.Filter.Interface
{
    public interface ICombatantTargetFinder
    {
        public IEnumerable<CombatantEntity> SelectPreferredTargets(TargetingPreference targetingPreference, CombatantStatType combatantStatType, TargetingType targetingType, TargetingType casterTargetingType, byte targetCount);
    }
}