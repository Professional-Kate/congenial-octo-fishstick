using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class CombatantEntityFactory : ICombatantEntityFactory
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CombatantEntityFactory(ICombatantRepository combatantRepository, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, IRepositoryAsserter repositoryAsserter)
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
                _collectionAssertion.AssertHasElements(combatantCard.SkillCards);
                
                CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard.StatCard)
                {
                    CombatantID = nextCombatantID,
                    IsFriendly = isFriendly,
                    CombatantType =  combatantCard.CombatantType,
                    CombatantInformation = combatantCard.Information
                };

                _combatantRepository.Add(combatantEntity);
            }
        }
    }
}