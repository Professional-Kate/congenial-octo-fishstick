using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantRepository : ICombatantRepository, ICombatantFilters
    {
        private readonly Dictionary<byte, CombatantEntity> _combatantRepository = [];
        private readonly IFoundAssertion _foundAssertion;

        public byte NextCombatantID { get; private set; }

        public CombatantRepository(IFoundAssertion foundAssertion)
        {
            _foundAssertion = foundAssertion;
        }

        public void Add(CombatantEntity combatantEntity)
        {
            _combatantRepository.Add(NextCombatantID, combatantEntity);
            NextCombatantID++;
        }

        public bool Contains(byte id)
        {
            return _combatantRepository.ContainsKey(id);
        }

        public void Clear()
        {
            _combatantRepository.Clear();
            
            NextCombatantID = 0;
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

        public IEnumerable<CombatantEntity> GetFriendlies()
        {
            return GetCombatants(true);
        }

        public IEnumerable<CombatantEntity> GetEnemies()
        {
            return GetCombatants(false);
        }

        private List<CombatantEntity> GetCombatants(bool isFriendly)
        {
            List<CombatantEntity> combatantEntities = [];
            foreach (CombatantEntity combatantEntity in _combatantRepository.Values)
            {
                if (combatantEntity.IsFriendly == isFriendly)
                {
                    combatantEntities.Add(combatantEntity);
                }
            }
            
            return combatantEntities;
        }
    }
}