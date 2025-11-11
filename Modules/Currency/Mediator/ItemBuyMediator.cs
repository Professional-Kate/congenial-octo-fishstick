using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency.Mediator
{
    public sealed class ItemBuyMediator : IBatchMediator<ItemBuy>
    { 
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly ICurrencyUpdateService _currencyUpdateService;
        private readonly IItemBuyResponseFactory _itemBuyResponseFactory;
        private readonly IDispatchMany<ItemBuyResponse> _itemBuyResponseDispatcher;
        private readonly IDispatchMany<CurrencyUpdateResponse> _currencyUpdateResponseDispatcher;
        private readonly IDispatchMany<InventoryUpdate> _inventoryUpdateDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IAmountAssertion _amountAssertion;

        public ItemBuyMediator(ICurrencyUpdateFactory currencyUpdateFactory, ICurrencyUpdateService currencyUpdateService, IItemBuyResponseFactory itemBuyResponseFactory, IDispatchMany<ItemBuyResponse> itemBuyResponseDispatcher, IDispatchMany<CurrencyUpdateResponse> currencyUpdateResponseDispatcher, IDispatchMany<InventoryUpdate> inventoryUpdateDispatcher, ICollectionAssertion collectionAssertion, IAmountAssertion amountAssertion)
        {
            _currencyUpdateFactory = currencyUpdateFactory;
            _currencyUpdateService = currencyUpdateService;
            _itemBuyResponseFactory = itemBuyResponseFactory;
            _itemBuyResponseDispatcher = itemBuyResponseDispatcher;
            _currencyUpdateResponseDispatcher = currencyUpdateResponseDispatcher;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _collectionAssertion = collectionAssertion;
            _amountAssertion = amountAssertion;
        }

        public void HandleMessages(IReadOnlyList<ItemBuy> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CurrencyUpdate[] currencyUpdates = new CurrencyUpdate[messages.Count];
            InventoryUpdate[] inventoryUpdates = new InventoryUpdate[messages.Count];
            
            for (int i = 0; i < messages.Count; i++)
            {
                ItemBuy itemBuy = messages[i];
                _amountAssertion.AssertAmountNotZero(itemBuy.Price);
                _amountAssertion.AssertAmountNotZero(itemBuy.Amount);
                
                currencyUpdates[i] = _currencyUpdateFactory.CreateCurrencyUpdate(ActionType.REMOVE, itemBuy.Price, itemBuy.CurrencyType);
                inventoryUpdates[i] = new InventoryUpdate { ActionType = ActionType.ADD, Amount = itemBuy.Amount, ItemID = itemBuy.ItemID };
            }

            IReadOnlyList<CurrencyUpdateResponse> currencyUpdateResponses = _currencyUpdateService.ApplyUpdates(currencyUpdates);
            
            _currencyUpdateResponseDispatcher.Dispatch(currencyUpdateResponses);
            _inventoryUpdateDispatcher.Dispatch(inventoryUpdates);
            _itemBuyResponseDispatcher.Dispatch(_itemBuyResponseFactory.CreateMultiple(messages));
        }
    }
}