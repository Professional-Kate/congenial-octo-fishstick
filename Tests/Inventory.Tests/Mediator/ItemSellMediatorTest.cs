using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests.Mediator
{
    [TestFixture]
    public sealed class ItemSellMediatorTest
    {
        private ItemSellMediator _itemSellMediator;
        private Mock<IInventoryUpdateService> _inventoryUpdateService;
        private Mock<IInventoryUpdateSummarizer> _updateSummarizer;
        private Mock<IInventoryUpdateFactory> _inventoryUpdateFactory;
        private Mock<IDispatchMany<InventoryUpdateResponse>> _inventoryUpdateDispatcherMock;
        private Mock<IDispatchOne<CurrencyUpdate>> _currencyUpdateDispatcherMock;
        private Mock<IDispatchMany<ItemSellResponse>> _itemSellDispatcherMock;

        private ItemSell _goldSell;
        private InventoryUpdate _goldUpdate;
        private InventoryUpdateResponse _goldUpdateResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _goldSell = new ItemSell { ItemID = ItemID.GOLD, Amount = 1 };

            _goldUpdate = new InventoryUpdate { ItemID = ItemID.GOLD, Amount = 1, ActionType = ActionType.ADD };
            _goldUpdateResponse = new InventoryUpdateResponse
            {
                ItemInfo = new ItemInfo { Amount = 1, BaseSellPrice = 1, ItemID = ItemID.GOLD, Information = new Information() { Name = "", Description = ""} },
                MutateType = MutateType.CHANGED
            };
            
            _inventoryUpdateService = new Mock<IInventoryUpdateService>();
            _updateSummarizer = new Mock<IInventoryUpdateSummarizer>();
            _inventoryUpdateFactory = new Mock<IInventoryUpdateFactory>();
            _inventoryUpdateDispatcherMock = new Mock<IDispatchMany<InventoryUpdateResponse>>();
            _currencyUpdateDispatcherMock = new Mock<IDispatchOne<CurrencyUpdate>>();
            _itemSellDispatcherMock = new Mock<IDispatchMany<ItemSellResponse>>();
            
            _itemSellMediator = new ItemSellMediator(_inventoryUpdateService.Object, _updateSummarizer.Object, _inventoryUpdateFactory.Object, _itemSellDispatcherMock.Object, _inventoryUpdateDispatcherMock.Object, _currencyUpdateDispatcherMock.Object, new CollectionAssertion(), new AmountAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _currencyUpdateDispatcherMock.Reset();
            _inventoryUpdateDispatcherMock.Reset();
            _itemSellDispatcherMock.Reset();
            _updateSummarizer.Reset();
        }

        private void SetupUpdateSummarizer(params InventoryUpdate[] summerizedUpdates)
        {
            _updateSummarizer.Setup(library => library.GetSummary(It.IsAny<InventoryUpdate[]>())).Returns(summerizedUpdates);
        }

        private void VerifyUpdateSummarizer()
        {
            _updateSummarizer.Verify(library => library.GetSummary(It.IsAny<InventoryUpdate[]>()), Times.Once);
        }

        private void SetupInventoryUpdateService(params InventoryUpdateResponse[] responses)
        {
            _inventoryUpdateService.Setup(library => library.ApplyUpdates(It.IsAny<InventoryUpdate[]>())).Returns(responses);
        }

        private void VerifyInventoryUpdateService(params InventoryUpdate[] updates)
        {
            _inventoryUpdateService.Verify(library => library.ApplyUpdates(updates), Times.Once);
        }

        private void VerifyCurrencyUpdateDispatched(uint amount)
        {
            _currencyUpdateDispatcherMock.Verify(library => library.Dispatch(It.Is<CurrencyUpdate>(update => update.Amount == amount)), Times.Once);
        }

        private void VerifyInventoryUpdateResponseDispatched(params InventoryUpdateResponse[] responses)
        {
            _inventoryUpdateDispatcherMock.Verify(library => library.Dispatch(responses), Times.Once);
        }

        private void VerifyItemSellResponseDispatched()
        {
            _itemSellDispatcherMock.Verify(library => library.Dispatch(It.IsAny<ItemSellResponse[]>()), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_SingleItem_RemovesItem_DispatchesUpdate()
        {
            SetupUpdateSummarizer(_goldUpdate);
            SetupInventoryUpdateService(_goldUpdateResponse);
            
            Assert.DoesNotThrow(() => _itemSellMediator.HandleMessages([_goldSell]));

            VerifyUpdateSummarizer();
            VerifyInventoryUpdateService(_goldUpdate);
            VerifyCurrencyUpdateDispatched(1);
            VerifyInventoryUpdateResponseDispatched(_goldUpdateResponse);
            VerifyItemSellResponseDispatched();
        }

        [Test]
        public void Positive_HandleMessages_MultipleItems_RemovesItems_DispatchesUpdate()
        {
            SetupUpdateSummarizer(_goldUpdate with { Amount = 3});
            SetupInventoryUpdateService(_goldUpdateResponse with { ItemInfo = _goldUpdateResponse.ItemInfo with { Amount = 3 } });
            
            Assert.DoesNotThrow(() => _itemSellMediator.HandleMessages([_goldSell, _goldSell, _goldSell]));

            VerifyUpdateSummarizer();
            VerifyInventoryUpdateService(_goldUpdate with { Amount = 3});
            VerifyCurrencyUpdateDispatched(3);
            VerifyInventoryUpdateResponseDispatched(_goldUpdateResponse with { ItemInfo = _goldUpdateResponse.ItemInfo with { Amount = 3 } });
            VerifyItemSellResponseDispatched();
        }

        [Test]
        public void Negative_HandleMessages_SingleItem_ZeroAmount_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _itemSellMediator.HandleMessages([_goldSell with { Amount = 0 }]));
        }
        
        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _itemSellMediator.HandleMessages([]));
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _itemSellMediator.HandleMessages(null!));
        }
    }
}