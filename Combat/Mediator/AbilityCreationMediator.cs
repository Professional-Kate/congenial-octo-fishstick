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
    public sealed class AbilityCreationMediator : IBatchMediator<AbilityCreation>
    {
        private readonly IAssetRepository<AbilityType, AbilityEntity> _skillEntityRepository;
        private readonly IAbilityEntityFactory _abilityEntityFactory;
        private readonly IDispatchMany<AbilityCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly INumberAssertion _numberAssertion;

        public AbilityCreationMediator(IAssetRepository<AbilityType, AbilityEntity> skillEntityRepository, IAbilityEntityFactory abilityEntityFactory, IDispatchMany<AbilityCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, INumberAssertion numberAssertion)
        {
            _skillEntityRepository = skillEntityRepository;
            _abilityEntityFactory = abilityEntityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
            _numberAssertion = numberAssertion;
        }

        public void HandleMessages(IReadOnlyList<AbilityCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            AbilityCreationResponse[] responses = new AbilityCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                AbilityCreation abilityCreation = messages[i];
                _numberAssertion.AssertNumberNotZero(abilityCreation.Cooldown, abilityCreation.ToString());
                _uniqueAssertion.AssertUnique(abilityCreation.AbilityType, _skillEntityRepository.Contains(abilityCreation.AbilityType));
                
                _skillEntityRepository.Add(abilityCreation.AbilityType, _abilityEntityFactory.CreateAbilityEntity(abilityCreation));
                responses[i] = CreateResponse(abilityCreation);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private static AbilityCreationResponse CreateResponse(AbilityCreation abilityCreation)
        {
            return new AbilityCreationResponse
            {
                Information = abilityCreation.Information, 
                AbilityType =  abilityCreation.AbilityType, 
                Cooldown =  abilityCreation.Cooldown,
                Damage = abilityCreation.Damage
            };
        }
    }
}