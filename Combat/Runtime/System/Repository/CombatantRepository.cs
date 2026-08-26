using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Repository
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

        public IEnumerable<CombatantEntity> GetAllParticipating()
        {
            List<CombatantEntity> combatantEntities = [];
            foreach (CombatantEntity combatantEntity in _combatantRepository.Values)
            {
                if (IsCombatantParticipating(combatantEntity) == false)
                {
                    continue;
                }
                
                combatantEntities.Add(combatantEntity);
            }

            return combatantEntities;
        }

        public bool HasValidCombatants(TargetingType targetingType)
        {
            foreach (CombatantEntity combatantEntity in _combatantRepository.Values)
            {
                if (IsCombatantParticipating(combatantEntity) == false)
                {
                    continue;
                }
                
                if (IsCombatantAlive(combatantEntity) == false)
                {
                    continue;
                }
                
                TargetingType combatantTargetingType = combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType;
                if (combatantTargetingType != targetingType)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public IReadOnlyList<CombatantEntity> GetCombatants(TargetingType targetingType, TargetingType casterTargetingType)
        {
            List<CombatantEntity> combatantEntities = [];
            foreach (CombatantEntity combatantEntity in _combatantRepository.Values)
            {
                if (IsCombatantParticipating(combatantEntity) == false)
                {
                    continue;
                }
                
                if (IsCombatantAlive(combatantEntity) == false)
                {
                    continue;
                }
                
                TargetingType combatantTargetingType = combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType;
                bool shouldTarget = targetingType switch
                {
                    TargetingType.FRIENDLY => combatantTargetingType == casterTargetingType,
                    TargetingType.ENEMY => combatantTargetingType != casterTargetingType,
                    _ => false
                };
                
                if (shouldTarget)
                {
                    combatantEntities.Add(combatantEntity);
                }
            }
            
            return combatantEntities.ToArray();
        }
        
        private static bool IsCombatantAlive(CombatantEntity combatantEntity) => combatantEntity.GetComponent<LifeStatusComponent>().IsAlive;
        
        private static bool IsCombatantParticipating(CombatantEntity combatantEntity) => combatantEntity.ContainsComponent<CombatParticipantComponent>();
    }
}