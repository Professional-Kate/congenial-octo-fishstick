using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantFactory : ICombatantFactory
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ISkillComponentFactory _skillComponentFactory;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CombatantFactory(ICombatantRepository combatantRepository, ISkillComponentFactory skillComponentFactory, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, IRepositoryAsserter repositoryAsserter)
        {
            _combatantRepository = combatantRepository;
            _skillComponentFactory = skillComponentFactory;
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
                
                CombatantEntity combatantEntity = new(_repositoryAsserter, combatantCard.StatCard, _skillComponentFactory.CreateMultiple(combatantCard.SkillCards))
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