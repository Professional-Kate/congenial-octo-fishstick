using IdelPog.Combat.Contracts.Command;
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
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CombatantEntityFactory(ICombatantRepository combatantRepository, IUniqueAssertion uniqueAssertion, IRepositoryAsserter repositoryAsserter)
        {
            _combatantRepository = combatantRepository;
            _repositoryAsserter = repositoryAsserter;
            _uniqueAssertion = uniqueAssertion;
        }

        public CombatantEntity CreateEntity(CombatantCreation combatantCreation)
        {
            byte nextCombatantID = _combatantRepository.NextCombatantID;
            _uniqueAssertion.AssertUnique(nextCombatantID, _combatantRepository.Contains(nextCombatantID));
            
            return new CombatantEntity(_repositoryAsserter, combatantCreation.StatCard)
            {
                CombatantID = nextCombatantID,
                CombatantType =  combatantCreation.CombatantType,
                CombatantInformation = combatantCreation.Information
            };
        }
    }
}