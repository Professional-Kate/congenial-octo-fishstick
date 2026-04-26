using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.System.Repository.Interface
{
    public interface ICombatantRepository
    {
        public byte NextCombatantID { get; }

        public void Add(CombatantEntity combatantEntity);

        public bool Contains(byte id);
        
        public void Clear();
        
        public CombatantEntity Get(byte id);

        public IEnumerable<CombatantEntity> GetAll();
    }
}