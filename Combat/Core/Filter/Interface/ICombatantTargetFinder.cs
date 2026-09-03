using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Filter.Interface
{
    public interface ICombatantTargetFinder
    {
        public IEnumerable<CombatantEntity> SelectPreferredTargets(TargetingPreference targetingPreference, CombatantStatType combatantStatType, TargetingType targetingType, TargetingType casterTargetingType, byte targetCount);
    }
}