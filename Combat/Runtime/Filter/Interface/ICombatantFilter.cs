namespace IdelPog.Combat.Runtime.Filter.Interface
{
    public interface ICombatantFilter
    { 
        public CombatantEntity GetEntity(IEnumerable<CombatantEntity> combatants);
    }
}