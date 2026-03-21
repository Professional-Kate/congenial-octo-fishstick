namespace IdelPog.Combat.Runtime.Filter.Interface
{
    public interface ICombatantSelector
    { 
        public CombatantEntity GetEntity(IEnumerable<CombatantEntity> combatants);
    }
}