using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Mediator
{
    public sealed class ItemSellMediator : IBatchMediator<ItemSell>
    {
        private readonly IAssetRepository<ItemID, ItemDefinition> _definitionRepository;
        private readonly IInventoryUpdateService _inventoryUpdateService;
        private readonly IInventoryUpdateSummarizer _updateSummarizer;
        private readonly IInventoryUpdateFactory _inventoryUpdateFactory;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;
        private readonly IDispatchMany<CurrencyUpdate> _currencyUpdateDispatcher;
        private readonly IDispatchMany<ItemSellResponse> _itemSellDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IAmountAssertion _amountAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public ItemSellMediator(IAssetRepository<ItemID, ItemDefinition> definitionRepository, IInventoryUpdateService inventoryUpdateService, IInventoryUpdateSummarizer updateSummarizer, IInventoryUpdateFactory inventoryUpdateFactory, IDispatchMany<ItemSellResponse> itemSellDispatcher, IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher, IDispatchMany<CurrencyUpdate> currencyUpdateDispatcher, ICollectionAssertion collectionAssertion, IAmountAssertion amountAssertion, IFoundAssertion foundAssertion)
        {
            _definitionRepository = definitionRepository;
            _inventoryUpdateService = inventoryUpdateService;
            _updateSummarizer = updateSummarizer;
            _inventoryUpdateFactory = inventoryUpdateFactory;
            _itemSellDispatcher = itemSellDispatcher;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _collectionAssertion = collectionAssertion;
            _amountAssertion = amountAssertion;
            _foundAssertion = foundAssertion;
        }

        public void HandleMessages(IReadOnlyList<ItemSell> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            ItemSellResponse[] responses = new ItemSellResponse[messages.Count];
            InventoryUpdate[] inventoryUpdates = new InventoryUpdate[messages.Count];
            CurrencyUpdate[] currencyUpdates = new CurrencyUpdate[messages.Count];
            
            for (var i = 0; i < messages.Count; i++)
            {
                ItemSell itemSell = messages[i];
                _amountAssertion.AssertAmountNotZero(itemSell.Amount);
                _foundAssertion.AssertFound(itemSell.ItemID, _definitionRepository.Contains(itemSell.ItemID));

                inventoryUpdates[i] = _inventoryUpdateFactory.Create(itemSell.ItemID, itemSell.Amount, ActionType.REMOVE);
                currencyUpdates[i] = CreateCurrencyUpdate(itemSell, _definitionRepository.Get(itemSell.ItemID).BaseSellPrice);
                responses[i] = new ItemSellResponse { CurrencyType = itemSell.CurrencyType, ItemID = itemSell.ItemID, Amount = itemSell.Amount };
            }

            DispatchInventoryUpdates(inventoryUpdates);
            
            _currencyUpdateDispatcher.Dispatch(currencyUpdates);
            _itemSellDispatcher.Dispatch(responses);
        }

        private void DispatchInventoryUpdates(IReadOnlyList<InventoryUpdate> updates)
        {
            IReadOnlyList<InventoryUpdate> summerizedUpdates = _updateSummarizer.GetSummary(updates);
            IReadOnlyList<InventoryUpdateResponse> inventoryUpdateResponses = _inventoryUpdateService.ApplyUpdates(summerizedUpdates);
            _inventoryUpdateDispatcher.Dispatch(inventoryUpdateResponses);
        }

        private static CurrencyUpdate CreateCurrencyUpdate(ItemSell itemSell, uint baseSellPrice)
        {
            return new CurrencyUpdate
            {
                CurrencyType = itemSell.CurrencyType,
                Amount = baseSellPrice * itemSell.Amount,
                ActionType = ActionType.ADD
            };
        }
    }
}