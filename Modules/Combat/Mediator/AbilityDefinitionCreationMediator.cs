using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class AbilityDefinitionCreationMediator : IBatchMediator<AbilityDefinitionCreation>
    {
        private readonly IAssetRepository<AbilityType, AbilityDefinition> _abilityRepository;
        private readonly IAbilityDefinitionFactory _abilityDefinitionFactory;
        private readonly IDispatchMany<AbilityDefinitionCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IAmountAssertion _amountAssertion;

        public AbilityDefinitionCreationMediator(IAssetRepository<AbilityType, AbilityDefinition> abilityRepository, IAbilityDefinitionFactory abilityDefinitionFactory, IDispatchMany<AbilityDefinitionCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, IAmountAssertion amountAssertion)
        {
            _abilityRepository = abilityRepository;
            _abilityDefinitionFactory = abilityDefinitionFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
            _amountAssertion = amountAssertion;
        }

        public void HandleMessages(IReadOnlyList<AbilityDefinitionCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            AbilityDefinitionCreationResponse[] responses = new AbilityDefinitionCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                AbilityDefinitionCreation creation = messages[i];
                
                _uniqueAssertion.AssertUnique(creation.AbilityType, _abilityRepository.Contains(creation.AbilityType));
                AssertCreation(creation);
                
                _abilityRepository.Add(creation.AbilityType, _abilityDefinitionFactory.Create(creation));

                responses[i] = new AbilityDefinitionCreationResponse
                {
                    AbilityType = creation.AbilityType,
                    TargetingInformation = creation.TargetingInformation,
                    Information = creation.Information,
                    Cooldown = creation.Cooldown,
                    Damage = creation.Damage
                };
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private void AssertCreation(AbilityDefinitionCreation abilityDefinitionCreation)
        { 
            _amountAssertion.AssertAmountNotZero(abilityDefinitionCreation.Damage);
            _amountAssertion.AssertAmountNotZero(abilityDefinitionCreation.TargetingInformation.MaxTargets);
        }
    }
}