using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    /// <summary>
    /// Filters for returning <see cref="CombatantEntity"/> based on their <see cref="TargetingType"/>.
    /// </summary>
    public interface ICombatantFilters
    {
        /// <summary>
        /// Returns whether a team of combatants has any valid entities that can continue fighting.
        /// </summary>
        /// <param name="targetingType">The team to query.</param>
        /// <returns>True if the team has any valid combatants</returns>
        public bool HasValidCombatants(TargetingType targetingType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetingType"></param>
        /// <param name="casterTargetingType"></param>
        /// <returns></returns>
        public IReadOnlyList<CombatantEntity> GetCombatants(TargetingType targetingType, TargetingType casterTargetingType);
    }
}