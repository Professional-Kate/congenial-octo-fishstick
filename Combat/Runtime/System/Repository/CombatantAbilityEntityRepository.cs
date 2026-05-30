using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Repository
{
    public sealed class CombatantAbilityEntityRepository : ICombatantAbilityEntityRepository
    {
        private readonly Dictionary<byte, IReadOnlyList<CombatantAbilityEntity>> _combatantAbilities = [];
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public CombatantAbilityEntityRepository(ICollectionAssertion collectionAssertion, IFoundAssertion foundAssertion)
        {
            _collectionAssertion = collectionAssertion;
            _foundAssertion = foundAssertion;
        }

        public void Add(byte combatantID, IReadOnlyList<CombatantAbilityEntity> combatantAbilities)
        {
            _collectionAssertion.AssertHasElements(combatantAbilities);
            _combatantAbilities.Add(combatantID, combatantAbilities);
        }

        public bool Contains(byte combatantID)
        {
            return _combatantAbilities.ContainsKey(combatantID);
        }

        public CombatantAbilityEntity Get(byte combatantID, AbilityType abilityType)
        { 
            _foundAssertion.AssertFound(combatantID, _combatantAbilities.ContainsKey(combatantID));

            foreach (CombatantAbilityEntity abilityEntity in _combatantAbilities[combatantID])
            {
                if (abilityEntity.AbilityType == abilityType)
                {
                    return abilityEntity;
                }
            }
            
            throw new KeyNotFoundException();
        }

        public IReadOnlyList<CombatantAbilityEntity> GetAll(byte combatantID)
        {
            _foundAssertion.AssertFound(combatantID, _combatantAbilities.ContainsKey(combatantID));
            return _combatantAbilities[combatantID];
        }
    }
}