namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ICombatantRepository
    {
        public void Add(CombatantEntity combatantEntity);

        public bool Contains(byte id);
        
        public void Clear();
    }
}