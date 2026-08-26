using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Repository
{
    public sealed class CombatantAbilityEntityRepository : ICombatantAbilityEntityRepository
    {
        private readonly Dictionary<byte, List<CombatantAbilityEntity>> _combatantAbilities = [];
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public CombatantAbilityEntityRepository(ICollectionAssertion collectionAssertion, IFoundAssertion foundAssertion)
        {
            _collectionAssertion = collectionAssertion;
            _foundAssertion = foundAssertion;
        }

        public void AddAbilities(byte combatantID, IReadOnlyList<CombatantAbilityEntity> combatantAbilities)
        {
            _collectionAssertion.AssertHasElements(combatantAbilities);
            if (_combatantAbilities.TryAdd(combatantID, [..combatantAbilities]))
            {
                return;
            }

            _combatantAbilities[combatantID].AddRange(combatantAbilities);
        }

        public bool Contains(byte combatantID)
        {
            return _combatantAbilities.ContainsKey(combatantID);
        }

        public CombatantAbilityEntity Get(byte combatantID, byte abilityID)
        { 
            _foundAssertion.AssertFound(combatantID, Contains(combatantID));

            foreach (CombatantAbilityEntity abilityEntity in _combatantAbilities[combatantID])
            {
                if (abilityEntity.AbilityID == abilityID)
                {
                    return abilityEntity;
                }
            }
            
            throw new KeyNotFoundException();
        }

        public IReadOnlyList<CombatantAbilityEntity> GetAll(byte combatantID)
        {
            if (Contains(combatantID) == false)
            {
                return [];
            }
            
            return _combatantAbilities[combatantID].AsReadOnly();
        }
    }
}