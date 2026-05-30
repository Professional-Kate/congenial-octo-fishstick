using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    /// <summary>
    /// Filters for returning <see cref="CombatantEntity"/> based on their <see cref="FriendlyStatusComponent"/>.
    /// </summary>
    public interface ICombatantFilters
    {
        /// <summary>
        /// Get all <see cref="CombatantEntity"/> whos <see cref="FriendlyStatusComponent"/> is true 
        /// </summary>
        /// <remarks>Will only return entities who's <see cref="LifeStatusComponent"/> is true</remarks>
        /// <returns>A collection of friendly entities</returns>
        public IEnumerable<CombatantEntity> GetFriendlies();
        
        /// <summary>
        /// Get all <see cref="CombatantEntity"/> whos <see cref="FriendlyStatusComponent"/> is false
        /// </summary>
        /// <remarks>Will only return entities who's <see cref="LifeStatusComponent"/> is true</remarks>
        /// <returns>A collection of enemy entities</returns>
        public IEnumerable<CombatantEntity> GetEnemies();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isFriendly"></param>
        /// <returns></returns>
        public IReadOnlyList<CombatantEntity> GetCombatants(bool isFriendly);
    }
}