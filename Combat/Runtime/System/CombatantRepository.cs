using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantRepository : ICombatantRepository
    {
        private readonly Dictionary<byte, CombatantEntity> _combatantRepository = [];

        private byte _nextID;

        public void Add(CombatantEntity combatantEntity)
        {
            _combatantRepository.Add(_nextID, combatantEntity);
            _nextID++;
        }

        public bool Contains(byte id)
        {
            return _combatantRepository.ContainsKey(id);
        }

        public void Clear()
        {
            _combatantRepository.Clear();
            
            _nextID = 0;
        }

        public IEnumerable<CombatantEntity> GetAll()
        {
            return _combatantRepository.Values;
        }
    }
}