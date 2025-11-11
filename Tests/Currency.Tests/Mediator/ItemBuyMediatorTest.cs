using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Mediator;
using IdelPog.Currency.Service.Interface;
using IdelPog.Inventory.Exceptions;
using Moq;

namespace IdelPog.Currency.Tests.Mediator
{
    [TestFixture]
    public sealed class ItemBuyMediatorTest
    {
        private ItemBuyMediator _itemBuyMediator;
        private Mock<ICurrencyUpdateFactory> _currencyUpdateFactoryMock;
        private Mock<ICurrencyUpdateService> _currencyUpdateServiceMock;
        private Mock<IItemBuyResponseFactory> _itemBuyResponseFactoryMock;
        private Mock<IDispatchMany<ItemBuyResponse>> _itemBuyResponseDispatcherMock;
        private Mock<IDispatchMany<CurrencyUpdateResponse>> _currencyUpdateResponseDispatcherMock;
        private Mock<IDispatchMany<InventoryUpdate>> _inventoryUpdateDispatcherMock;

        private ItemBuy _honeyBuy;
        private ItemBuyResponse _honeyBuyResponse;
        private CurrencyUpdate _removeGoldUpdate;
        private CurrencyUpdateResponse _removeGoldResponse;
        private InventoryUpdate _removeHoneyUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _currencyUpdateFactoryMock = new Mock<ICurrencyUpdateFactory>();
            _currencyUpdateServiceMock = new Mock<ICurrencyUpdateService>();
            _itemBuyResponseFactoryMock = new Mock<IItemBuyResponseFactory>();
            _itemBuyResponseDispatcherMock = new Mock<IDispatchMany<ItemBuyResponse>>();
            _currencyUpdateResponseDispatcherMock = new Mock<IDispatchMany<CurrencyUpdateResponse>>();
            _inventoryUpdateDispatcherMock = new Mock<IDispatchMany<InventoryUpdate>>();
            
            _itemBuyMediator = new ItemBuyMediator(_currencyUpdateFactoryMock.Object, _currencyUpdateServiceMock.Object, _itemBuyResponseFactoryMock.Object, _itemBuyResponseDispatcherMock.Object, _currencyUpdateResponseDispatcherMock.Object, _inventoryUpdateDispatcherMock.Object, new CollectionAssertion(), new AmountAssertion());

            _honeyBuy = new ItemBuy { CurrencyType = CurrencyType.GOLD, ItemID = ItemID.HONEY, Price = 1, Amount = 3 };
            _honeyBuyResponse = new ItemBuyResponse { CurrencyType = _honeyBuy.CurrencyType, ItemID = _honeyBuy.ItemID, Price = _honeyBuy.Price, Amount = _honeyBuy.Amount };
            _removeGoldUpdate = new CurrencyUpdate { CurrencyType = CurrencyType.GOLD, Amount = 1, ActionType = ActionType.REMOVE };
            _removeGoldResponse = new CurrencyUpdateResponse { CurrencyType = _removeGoldUpdate.CurrencyType, CurrencyAmount = 0 };
            _removeHoneyUpdate = new InventoryUpdate { ItemID = ItemID.HONEY, Amount = _honeyBuy.Amount, ActionType = ActionType.ADD };
        }

        private void SetupItemBuyResponseFactory(ItemBuy[] itemBuys, ItemBuyResponse[] itemBuyResponses)
        {
            _itemBuyResponseFactoryMock.Setup(library => library.CreateMultiple(itemBuys)).Returns(itemBuyResponses);
        }
        
        private void SetupCurrencyUpdateFactory(CurrencyUpdate currencyUpdates)
        {
            _currencyUpdateFactoryMock.Setup(library => library.CreateCurrencyUpdate(ActionType.REMOVE, currencyUpdates.Amount, currencyUpdates.CurrencyType)).Returns(currencyUpdates);
        }
        
        private void SetupCurrencyUpdateService(CurrencyUpdateResponse[] currencyUpdateResponses, CurrencyUpdate[] currencyUpdates)
        {
            _currencyUpdateServiceMock.Setup(library => library.ApplyUpdates(currencyUpdates)).Returns(currencyUpdateResponses);
        }
        
        private void VerifyItemBuyResponseDispatcher(params ItemBuyResponse[] itemBuyResponses)
        {
            _itemBuyResponseDispatcherMock.Verify(library => library.Dispatch(itemBuyResponses), Times.Once);
            _itemBuyResponseDispatcherMock.VerifyNoOtherCalls();
        }

        private void VerifyCurrencyUpdateDispatcher(params CurrencyUpdateResponse[] currencyUpdateResponses)
        {
            _currencyUpdateResponseDispatcherMock.Verify(library => library.Dispatch(currencyUpdateResponses), Times.Once);
            _currencyUpdateResponseDispatcherMock.VerifyNoOtherCalls();
        }

        private void VerifyInventoryUpdateDispatcher(params InventoryUpdate[] inventoryUpdates)
        {
            _inventoryUpdateDispatcherMock.Verify(library => library.Dispatch(inventoryUpdates), Times.Once);
            _inventoryUpdateDispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_RemovesCurrency_AddsItem_DispatchesAll()
        {
            SetupItemBuyResponseFactory([_honeyBuy], [_honeyBuyResponse]);
            SetupCurrencyUpdateFactory(_removeGoldUpdate);
            SetupCurrencyUpdateService([_removeGoldResponse], [_removeGoldUpdate]);
            
            Assert.DoesNotThrow(() => _itemBuyMediator.HandleMessages([_honeyBuy]));

            VerifyItemBuyResponseDispatcher(_honeyBuyResponse);
            VerifyCurrencyUpdateDispatcher(_removeGoldResponse);
            VerifyInventoryUpdateDispatcher(_removeHoneyUpdate);
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _itemBuyMediator.HandleMessages([]));
            
            _itemBuyResponseDispatcherMock.VerifyNoOtherCalls();
            _currencyUpdateResponseDispatcherMock.VerifyNoOtherCalls();
            _inventoryUpdateDispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        { 
            Assert.Throws<ArgumentNullException>(() => _itemBuyMediator.HandleMessages(null!));
            
            _itemBuyResponseDispatcherMock.VerifyNoOtherCalls();
            _currencyUpdateResponseDispatcherMock.VerifyNoOtherCalls();
            _inventoryUpdateDispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_ZeroPrice_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _itemBuyMediator.HandleMessages([_honeyBuy with { Price = 0 }]));
            
            _itemBuyResponseDispatcherMock.VerifyNoOtherCalls();
            _currencyUpdateResponseDispatcherMock.VerifyNoOtherCalls();
            _inventoryUpdateDispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_ZeroAmount_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _itemBuyMediator.HandleMessages([_honeyBuy with { Amount = 0 }]));
            
            _itemBuyResponseDispatcherMock.VerifyNoOtherCalls();
            _currencyUpdateResponseDispatcherMock.VerifyNoOtherCalls();
            _inventoryUpdateDispatcherMock.VerifyNoOtherCalls();
        }
    }
}