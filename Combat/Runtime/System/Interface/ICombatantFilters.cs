namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ICombatantFilters
    {
        public IEnumerable<CombatantEntity> GetFriendlies();
        
        public IEnumerable<CombatantEntity> GetEnemies();
    }
}