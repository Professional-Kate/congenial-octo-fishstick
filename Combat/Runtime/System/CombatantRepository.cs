using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantRepository : ICombatantRepository
    {
        private readonly Dictionary<byte, CombatantEntity> _combatantRepository = [];
        private readonly IFoundAssertion _foundAssertion;

        private byte _nextID;

        public CombatantRepository(IFoundAssertion foundAssertion)
        {
            _foundAssertion = foundAssertion;
        }

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

        public CombatantEntity Get(byte id)
        { 
            _foundAssertion.AssertFound(id, _combatantRepository.ContainsKey(id));
            return _combatantRepository[id];
        }

        public IEnumerable<CombatantEntity> GetAll()
        {
            return _combatantRepository.Values;
        }
    }
}