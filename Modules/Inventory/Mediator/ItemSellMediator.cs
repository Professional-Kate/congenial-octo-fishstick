using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Mediator
{
    public sealed class ItemSellMediator : IBatchMediator<ItemSell>
    {
        private readonly IInventoryUpdateService _inventoryUpdateService;
        private readonly IInventoryUpdateSummarizer _updateSummarizer;
        private readonly IInventoryUpdateFactory _inventoryUpdateFactory;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;
        private readonly IDispatchOne<CurrencyUpdate> _currencyUpdateDispatcher;
        private readonly IDispatchMany<ItemSellResponse> _itemSellDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IAmountAssertion _amountAssertion;

        public ItemSellMediator(IInventoryUpdateService inventoryUpdateService, IInventoryUpdateSummarizer updateSummarizer, IInventoryUpdateFactory inventoryUpdateFactory, IDispatchMany<ItemSellResponse> itemSellDispatcher, IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher, IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher, ICollectionAssertion collectionAssertion, IAmountAssertion amountAssertion)
        {
            _inventoryUpdateService = inventoryUpdateService;
            _updateSummarizer = updateSummarizer;
            _inventoryUpdateFactory = inventoryUpdateFactory;
            _itemSellDispatcher = itemSellDispatcher;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _collectionAssertion = collectionAssertion;
            _amountAssertion = amountAssertion;
        }

        public void HandleMessages(IReadOnlyList<ItemSell> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            ItemSellResponse[] responses = new ItemSellResponse[messages.Count];
            InventoryUpdate[] inventoryUpdates = new InventoryUpdate[messages.Count];
            
            for (var i = 0; i < messages.Count; i++)
            {
                ItemSell itemSell = messages[i];
                _amountAssertion.AssertAmountNotZero(itemSell.Amount);

                inventoryUpdates[i] = _inventoryUpdateFactory.Create(itemSell.ItemID, itemSell.Amount, ActionType.REMOVE);
                
                responses[i] = new ItemSellResponse { ItemID = itemSell.ItemID, Amount = itemSell.Amount };
            }

            IReadOnlyList<InventoryUpdateResponse> inventoryUpdateResponses = DispatchInventoryUpdates(inventoryUpdates);
            DispatchCurrencyUpdate(inventoryUpdateResponses);
            
            _itemSellDispatcher.Dispatch(responses);
        }

        private IReadOnlyList<InventoryUpdateResponse> DispatchInventoryUpdates(IReadOnlyList<InventoryUpdate> updates)
        {
            IReadOnlyList<InventoryUpdate> summerizedUpdates = _updateSummarizer.GetSummary(updates);
            IReadOnlyList<InventoryUpdateResponse> inventoryUpdateResponses = _inventoryUpdateService.ApplyUpdates(summerizedUpdates);
            _inventoryUpdateDispatcher.Dispatch(inventoryUpdateResponses);
            
            return inventoryUpdateResponses;
        }

        private void DispatchCurrencyUpdate(IReadOnlyList<InventoryUpdateResponse> inventoryUpdateResponses)
        {
            uint total = 0;
            foreach (InventoryUpdateResponse inventoryUpdateResponse in inventoryUpdateResponses)
            {
                total += inventoryUpdateResponse.ItemInfo.BaseSellPrice * inventoryUpdateResponse.ItemInfo.Amount;
            }
            
            _amountAssertion.AssertAmountNotZero(total);
            _currencyUpdateDispatcher.Dispatch(new CurrencyUpdate { CurrencyType = CurrencyType.GOLD, Amount = total, ActionType = ActionType.ADD });
        }
    }
}