using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Interface
{
    /// <summary>
    /// Filters for returning <see cref="CombatantEntity"/> based on their <see cref="CombatantEntity.IsFriendly"/>.
    /// </summary>
    public interface ICombatantFilters
    {
        /// <summary>
        /// Get all <see cref="CombatantEntity"/> whos <see cref="CombatantEntity.IsFriendly"/> is true 
        /// </summary>
        /// <remarks>Will only return entities who's <see cref="CombatantEntity.IsAlive"/> is true</remarks>
        /// <returns>A collection of friendly entities</returns>
        public IEnumerable<CombatantEntity> GetFriendlies();
        
        /// <summary>
        /// Get all <see cref="CombatantEntity"/> whos <see cref="CombatantEntity.IsFriendly"/> is false
        /// </summary>
        /// <remarks>Will only return entities who's <see cref="CombatantEntity.IsAlive"/> is true</remarks>
        /// <returns>A collection of enemy entities</returns>
        public IEnumerable<CombatantEntity> GetEnemies();
    }
}