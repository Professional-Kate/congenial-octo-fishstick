using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantFactory : ICombatantFactory
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CombatantFactory(ICombatantRepository combatantRepository, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, IRepositoryAsserter repositoryAsserter)
        {
            _combatantRepository = combatantRepository;
            _collectionAssertion = collectionAssertion;
            _repositoryAsserter = repositoryAsserter;
            _uniqueAssertion = uniqueAssertion;
        }
        
        public void SpawnCombatants(IReadOnlyList<CombatantCard> combatants, bool isFriendly)
        {
            _collectionAssertion.AssertHasElements(combatants);
            
            foreach (CombatantCard combatantCard in combatants)
            {
                byte nextCombatantID = _combatantRepository.NextCombatantID;
                _uniqueAssertion.AssertUnique(nextCombatantID, _combatantRepository.Contains(nextCombatantID));
                
                CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard)
                {
                    CombatantID = nextCombatantID,
                    IsFriendly = isFriendly
                };

                _combatantRepository.Add(combatantEntity);
            }
        }
    }
}