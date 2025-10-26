using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Contracts;
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
        private Mock<IDispatchMany<CurrencyUpdate>> _currencyUpdateDispatcherMock;
        private Mock<IDispatchMany<ItemSellResponse>> _itemSellDispatcherMock;
        private Mock<IAssetRepository<ItemID, ItemDefinition>> _definitionRepositoryMock;

        private ItemSell _goldSell;
        private InventoryUpdate _goldUpdate;
        private InventoryUpdateResponse _goldUpdateResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _goldSell = new ItemSell { CurrencyType = CurrencyType.GOLD, ItemID = ItemID.GOLD, Amount = 1 };

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
            _currencyUpdateDispatcherMock = new Mock<IDispatchMany<CurrencyUpdate>>();
            _itemSellDispatcherMock = new Mock<IDispatchMany<ItemSellResponse>>();
            _definitionRepositoryMock = new Mock<IAssetRepository<ItemID, ItemDefinition>>();
            
            _itemSellMediator = new ItemSellMediator(_definitionRepositoryMock.Object, _inventoryUpdateService.Object, _updateSummarizer.Object, _inventoryUpdateFactory.Object, _itemSellDispatcherMock.Object, _inventoryUpdateDispatcherMock.Object, _currencyUpdateDispatcherMock.Object, new CollectionAssertion(), new AmountAssertion(), new FoundAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _currencyUpdateDispatcherMock.Reset();
            _inventoryUpdateDispatcherMock.Reset();
            _itemSellDispatcherMock.Reset();
            _updateSummarizer.Reset();
            _definitionRepositoryMock.Reset();
        }

        private void SetupUpdateSummarizer(params InventoryUpdate[] summerizedUpdates)
        {
            _updateSummarizer.Setup(library => library.GetSummary(It.IsAny<InventoryUpdate[]>())).Returns(summerizedUpdates);
        }

        private void SetupDefinitionRepository(ItemID itemID)
        {
            _definitionRepositoryMock.Setup(library => library.Contains(itemID)).Returns(true);
        }

        private void VerifyDefinitionRepository(ItemID itemID, Times times)
        {
            _definitionRepositoryMock.Verify(library => library.Contains(itemID), times);
            _definitionRepositoryMock.Verify(library => library.Get(itemID), times);
            _definitionRepositoryMock.VerifyNoOtherCalls();
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

        private void VerifyCurrencyUpdateDispatched(int length)
        {
            _currencyUpdateDispatcherMock.Verify(library => library.Dispatch(It.Is<CurrencyUpdate[]>(collection => collection.Length == length)), Times.Once);
        }

        private void VerifyInventoryUpdateResponseDispatched(params InventoryUpdateResponse[] responses)
        {
            _inventoryUpdateDispatcherMock.Verify(library => library.Dispatch(responses), Times.Once);
        }

        private void VerifyItemSellResponseDispatched(int length)
        {
            _itemSellDispatcherMock.Verify(library => library.Dispatch(It.Is<ItemSellResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_SingleItem_RemovesItem_DispatchesUpdate()
        {
            SetupDefinitionRepository(_goldUpdate.ItemID);
            SetupUpdateSummarizer(_goldUpdate);
            SetupInventoryUpdateService(_goldUpdateResponse);
            
            Assert.DoesNotThrow(() => _itemSellMediator.HandleMessages([_goldSell]));

            VerifyDefinitionRepository(_goldUpdate.ItemID, Times.Once());
            VerifyUpdateSummarizer();
            VerifyInventoryUpdateService(_goldUpdate);
            VerifyCurrencyUpdateDispatched(1);
            VerifyInventoryUpdateResponseDispatched(_goldUpdateResponse);
            VerifyItemSellResponseDispatched(1);
        }

        [Test]
        public void Positive_HandleMessages_MultipleItems_RemovesItems_DispatchesUpdate()
        {
            SetupDefinitionRepository(_goldUpdate.ItemID);
            SetupUpdateSummarizer(_goldUpdate with { Amount = 3});
            SetupInventoryUpdateService(_goldUpdateResponse with { ItemInfo = _goldUpdateResponse.ItemInfo with { Amount = 3 } });
            
            Assert.DoesNotThrow(() => _itemSellMediator.HandleMessages([_goldSell, _goldSell, _goldSell]));

            VerifyDefinitionRepository(_goldUpdate.ItemID, Times.Exactly(3));
            VerifyUpdateSummarizer();
            VerifyInventoryUpdateService(_goldUpdate with { Amount = 3});
            VerifyCurrencyUpdateDispatched(3);
            VerifyInventoryUpdateResponseDispatched(_goldUpdateResponse with { ItemInfo = _goldUpdateResponse.ItemInfo with { Amount = 3 } });
            VerifyItemSellResponseDispatched(3);
        }

        [Test]
        public void Positive_HandleMessages_MultipleCurrency_DispatchesCurrencyUpdates()
        {
            SetupDefinitionRepository(_goldUpdate.ItemID);
            SetupUpdateSummarizer(_goldUpdate with { Amount = 2 });
            SetupInventoryUpdateService(_goldUpdateResponse with { ItemInfo = _goldUpdateResponse.ItemInfo with { Amount = 2 }});
            
            ItemDefinition itemDefinition = new() { ItemID = ItemID.GOLD, BaseSellPrice = 1, Information = new Information { Name = "", Description = "" } };
            _definitionRepositoryMock.Setup(library => library.Get(_goldUpdate.ItemID)).Returns(itemDefinition);
            
            Assert.DoesNotThrow(() => _itemSellMediator.HandleMessages([_goldSell, _goldSell with { CurrencyType = CurrencyType.GEMS }]));
            
            VerifyDefinitionRepository(_goldUpdate.ItemID, Times.Exactly(2));
            VerifyUpdateSummarizer();
            VerifyInventoryUpdateService(_goldUpdate with { Amount = 2 });
            VerifyInventoryUpdateResponseDispatched(_goldUpdateResponse with { ItemInfo = _goldUpdateResponse.ItemInfo with { Amount = 2 } });
            VerifyItemSellResponseDispatched(2);
            
            VerifyCurrencyUpdateDispatched(2);
            CurrencyUpdate[] expectedUpdates =
            [
                new() { Amount = 1, CurrencyType = CurrencyType.GOLD, ActionType = ActionType.ADD },
                new() { Amount = 1, CurrencyType = CurrencyType.GEMS, ActionType = ActionType.ADD }
            ];
            
            _currencyUpdateDispatcherMock.Verify(library => library.Dispatch(expectedUpdates), Times.Once);
        }

        [Test]
        public void Negative_HandleMessages_DefinitionNotFound_Throws()
        { 
            Assert.Throws<NotFoundException<ItemID>>(() => _itemSellMediator.HandleMessages([_goldSell]));
            
            _definitionRepositoryMock.Verify(library => library.Contains(_goldSell.ItemID), Times.Once);
            _definitionRepositoryMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_SingleItem_ZeroAmount_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _itemSellMediator.HandleMessages([_goldSell with { Amount = 0 }]));
            
            _definitionRepositoryMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _itemSellMediator.HandleMessages([]));
            
            _definitionRepositoryMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _itemSellMediator.HandleMessages(null!));
            
            _definitionRepositoryMock.VerifyNoOtherCalls();
        }
    }
}