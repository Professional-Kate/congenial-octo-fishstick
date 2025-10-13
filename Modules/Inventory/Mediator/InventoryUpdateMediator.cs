using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Mediator
{
    public sealed class InventoryUpdateMediator : IBatchMediator<InventoryUpdate>
    {
        private readonly IInventoryUpdateService _inventoryUpdateService;
        private readonly IInventoryUpdateSummarizer _updateSummarizer;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public InventoryUpdateMediator(IInventoryUpdateService inventoryUpdateService, IInventoryUpdateSummarizer updateSummarizer, IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher, ICollectionAssertion collectionAssertion)
        {
            _inventoryUpdateService = inventoryUpdateService;
            _updateSummarizer = updateSummarizer;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<InventoryUpdate> updates)
        {
            _collectionAssertion.AssertHasElements(updates);

            IReadOnlyList<InventoryUpdate> summerizedUpdates = _updateSummarizer.GetSummary(updates);
            _collectionAssertion.AssertHasElements(summerizedUpdates);
            
             IReadOnlyList<InventoryUpdateResponse> responses = _inventoryUpdateService.ApplyUpdates(summerizedUpdates);
             _inventoryUpdateDispatcher.Dispatch(responses);
        }
    }
}