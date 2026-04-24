using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class CombatantAbilityCreationMediator : IBatchMediator<CombatantAbilityCreation>
    {
        private readonly IAssetRepository<AbilityType, AbilityEntity> _skillEntityRepository;
        private readonly IAbilityEntityFactory _abilityEntityFactory;
        private readonly IDispatchMany<CombatantAbilityCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly INumberAssertion _numberAssertion;

        public CombatantAbilityCreationMediator(IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository, IAbilityEntityFactory abilityEntityFactory, IDispatchMany<CombatantAbilityCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, INumberAssertion numberAssertion)
        {
            _skillEntityRepository = skillEntityRepository;
            _abilityEntityFactory = abilityEntityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
            _numberAssertion = numberAssertion;
        }

        public void HandleMessages(IReadOnlyList<CombatantAbilityCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CombatantAbilityCreationResponse[] responses = new CombatantAbilityCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                CombatantAbilityCreation combatantAbilityCreation = messages[i];
                _numberAssertion.AssertNumberNotZero(combatantAbilityCreation.Speed, combatantAbilityCreation.ToString());
                _uniqueAssertion.AssertUnique(combatantAbilityCreation.AbilityType, _skillEntityRepository.Contains(combatantAbilityCreation.AbilityType));
                
                _skillEntityRepository.Add(combatantAbilityCreation.AbilityType, _abilityEntityFactory.CreateAbilityEntity(combatantAbilityCreation));
                responses[i] = CreateResponse(combatantAbilityCreation);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private static CombatantAbilityCreationResponse CreateResponse(CombatantAbilityCreation combatantAbilityCreation)
        {
            return new CombatantAbilityCreationResponse
            {
                Information = combatantAbilityCreation.Information, 
                AbilityType =  combatantAbilityCreation.AbilityType, 
                Speed =  combatantAbilityCreation.Speed,
                Damage = combatantAbilityCreation.Damage
            };
        }
    }
}