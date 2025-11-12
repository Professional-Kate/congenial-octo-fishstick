using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;

namespace IdelPog.Inventory.Mediator
{
    public sealed class ItemDefinitionCreationMediator : IBatchMediator<ItemDefinitionCreation>
    {
        private readonly IAssetRepository<ItemID, ItemDefinition> _definitionRepository;
        private readonly IDispatchMany<ItemDefinitionCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IAmountAssertion _amountAssertion;

        public ItemDefinitionCreationMediator(IAssetRepository<ItemID, ItemDefinition> definitionRepository, IDispatchMany<ItemDefinitionCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, IAmountAssertion amountAssertion)
        {
            _definitionRepository = definitionRepository;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
            _amountAssertion = amountAssertion;
        }

        public void HandleMessages(IReadOnlyList<ItemDefinitionCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            ItemDefinitionCreationResponse[] responses = new ItemDefinitionCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                ItemDefinitionCreation itemDefinitionCreation = messages[i];
                _amountAssertion.AssertAmountNotZero(itemDefinitionCreation.BaseSellPrice);
                _uniqueAssertion.AssertUnique(itemDefinitionCreation.ItemID, _definitionRepository.Contains(itemDefinitionCreation.ItemID));

                ItemDefinition definition = new() { ItemID = itemDefinitionCreation.ItemID, BaseSellPrice = itemDefinitionCreation.BaseSellPrice, Information = itemDefinitionCreation.Information };
                _definitionRepository.Add(definition.ItemID, definition);
                
                ItemDefinitionCreationResponse response = new() { ItemID = itemDefinitionCreation.ItemID, BaseSellPrice = itemDefinitionCreation.BaseSellPrice, Information = itemDefinitionCreation.Information };
                responses[i] = response;
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}