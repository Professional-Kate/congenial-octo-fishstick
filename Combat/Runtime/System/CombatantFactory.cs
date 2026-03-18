using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantFactory : ICombatantFactory
    {
        private readonly IAssetRepository<byte, CombatantEntity> _combatantRepository;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CombatantFactory(IAssetRepository<byte, CombatantEntity> combatantRepository, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, IRepositoryAsserter repositoryAsserter)
        {
            _combatantRepository = combatantRepository;
            _collectionAssertion = collectionAssertion;
            _repositoryAsserter = repositoryAsserter;
            _uniqueAssertion = uniqueAssertion;
        }
        
        public void SpawnCombatants(IReadOnlyList<CombatantCard> combatants)
        {
            _collectionAssertion.AssertHasElements(combatants);
            
            for (byte i = 0; i < combatants.Count; i++)
            {
                _uniqueAssertion.AssertUnique(i, _combatantRepository.Contains(i));
                
                CombatantCard combatantCard = combatants[i];
                CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard.StatCard) {  IsFriendly = combatantCard.IsFriendly };

                _combatantRepository.Add(i, combatantEntity);
            } 
        }
    }
}