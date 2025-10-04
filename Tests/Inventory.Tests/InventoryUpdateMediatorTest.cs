using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests
{
    [TestFixture]
    public class InventoryUpdateMediatorTest
    {
        private IBatchMediator<InventoryUpdate> _inventoryMediator;
        private Mock<IInventory> _repositoryMock;
        private Mock<IItemFactory> _itemFactoryMock;
        private Mock<IInventoryUpdateSummarizer> _updateSummarizerMock;
        private Mock<IInventoryUpdateResponseFactory> _responseFactoryMock;
        private Mock<IItemInfoFactory> _itemInfoFactoryMock;
        private Mock<IInventoryUpdateEntryFactory> _entryFactoryMock;
        private Mock<IDispatchOne<InventoryUpdateResponse>> _dispatcherMock;
        private Mock<IMapper<ItemID>> _itemMapperMock;

        private Item _stoneItem;
        private InventoryUpdate _addStoneUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IInventory>();
            _itemFactoryMock = new Mock<IItemFactory>();
            _updateSummarizerMock = new Mock<IInventoryUpdateSummarizer>();
            _responseFactoryMock = new Mock<IInventoryUpdateResponseFactory>();
            _itemInfoFactoryMock = new Mock<IItemInfoFactory>();
            _entryFactoryMock = new Mock<IInventoryUpdateEntryFactory>();
            _dispatcherMock = new Mock<IDispatchOne<InventoryUpdateResponse>>();
            _itemMapperMock = new Mock<IMapper<ItemID>>();

            _inventoryMediator = new InventoryUpdateMediator(_repositoryMock.Object, _itemFactoryMock.Object, _updateSummarizerMock.Object, _responseFactoryMock.Object,
                _itemInfoFactoryMock.Object, _itemMapperMock.Object, _entryFactoryMock.Object, _dispatcherMock.Object, new CollectionAssertion(new ThrowHandler()));

            _stoneItem = new Item(ItemID.STONE, 1, new Information { Description = "", Name = "" }, 1);

            _addStoneUpdate = new InventoryUpdate
            {
                ActionType = ActionType.ADD,
                Amount = 1,
                ItemID = ItemID.STONE
            };
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _itemFactoryMock.Reset();
            _responseFactoryMock.Reset();
            _dispatcherMock.Reset();
            _updateSummarizerMock.Reset();
            _itemInfoFactoryMock.Reset();
            
            _repositoryMock.Setup(library => library.GetItem(_addStoneUpdate.ItemID)).Returns(_stoneItem);
        }

        private void SetupRepositoryContains(bool wantedContains)
        {
            _repositoryMock.Setup(library => library.Contains(_addStoneUpdate.ItemID)).Returns(wantedContains);
        }

        private void SetupSummarizerMock(InventoryUpdate[] inputUpdates, InventoryUpdate[] summaryUpdates)
        {
            _updateSummarizerMock.Setup(library => library.GetSummary(inputUpdates)).Returns(summaryUpdates);
        }

        private void VerifySingleCalls()
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdateResponse>()), Times.Once);
            _responseFactoryMock.Verify(library => library.Create(It.IsAny<InventoryUpdateEntry[]>()), Times.Once);
        }

        private void VerifyItemFactoryCalls(Times times)
        {
            _itemFactoryMock.Verify(library => library.CreateItem(_addStoneUpdate.ItemID, _addStoneUpdate.Amount), times);
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_CreateItem()
        {
            SetupSummarizerMock([_addStoneUpdate], [_addStoneUpdate]);
            SetupRepositoryContains(false);
            _itemFactoryMock.Setup(library => library.CreateItem(_stoneItem.ItemID, _stoneItem.Amount)).Returns(_stoneItem);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate]));

            VerifyItemFactoryCalls(Times.Once());
            _repositoryMock.Verify(library => library.Contains(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.AddItem(It.Is<Item>(item => item.ItemID == _addStoneUpdate.ItemID)), Times.Once);
            _repositoryMock.Verify(library => library.GetItem(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();

            VerifySingleCalls();
            _itemInfoFactoryMock.Verify(library => library.Create(_addStoneUpdate.ItemID, _stoneItem.BaseSellPrice, 1, It.IsAny<Information>()), Times.Once);
            _itemInfoFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_AddsToItem_NoCreation()
        {
            SetupSummarizerMock([_addStoneUpdate], [_addStoneUpdate]);
            SetupRepositoryContains(true);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate]));
            
            VerifyItemFactoryCalls(Times.Never());
            _repositoryMock.Verify(library => library.Contains(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.GetItem(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.AddAmount(_stoneItem.ItemID, _stoneItem.Amount), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
            
            VerifySingleCalls();
            _itemInfoFactoryMock.Verify(library => library.Create(_addStoneUpdate.ItemID, _stoneItem.BaseSellPrice, 1, It.IsAny<Information>()), Times.Once);
            _itemInfoFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_SingleItemID_DispatchesOneCombinedUpdate()
        {
            SetupSummarizerMock([_addStoneUpdate, _addStoneUpdate, _addStoneUpdate], [_addStoneUpdate with { Amount = 3 }]);
            SetupRepositoryContains(true);
            
            Item stone = new(ItemID.STONE, 1, new Information { Description = "", Name = "" }, 3);
            _repositoryMock.Setup(library => library.GetItem(_addStoneUpdate.ItemID)).Returns(stone);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate, _addStoneUpdate, _addStoneUpdate]));
            
            VerifyItemFactoryCalls(Times.Never());
            _repositoryMock.Verify(library => library.Contains(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.GetItem(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.AddAmount(_stoneItem.ItemID, 3), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
            
            VerifySingleCalls();
            _itemInfoFactoryMock.Verify(library => library.Create(_addStoneUpdate.ItemID, _stoneItem.BaseSellPrice, 3, It.IsAny<Information>()), Times.Once);
            _itemInfoFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_RemoveItem()
        {
            InventoryUpdate removeStoneUpdate = _addStoneUpdate with { ActionType = ActionType.REMOVE };
            SetupSummarizerMock([removeStoneUpdate], [removeStoneUpdate]);
            SetupRepositoryContains(true);

            _repositoryMock.Setup(library => library.RemoveAmount(removeStoneUpdate.ItemID, removeStoneUpdate.Amount)).Returns(MutateType.DELETED);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([removeStoneUpdate]));
            
            VerifyItemFactoryCalls(Times.Never());
            _repositoryMock.Verify(library => library.RemoveAmount(removeStoneUpdate.ItemID, removeStoneUpdate.Amount), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
            
            _itemInfoFactoryMock.Verify(library => library.Create(removeStoneUpdate.ItemID, 0, 0, It.IsAny<Information>()), Times.Once);
            _itemInfoFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleItemIDs_InvokesTwice_DispatchesOnce()
        {
            InventoryUpdate addGoldUpdate = _addStoneUpdate with { ItemID = ItemID.GOLD };
            Item gold = new(ItemID.GOLD, 2, new  Information { Description = "", Name = "" }, 1);
            SetupSummarizerMock([_addStoneUpdate, addGoldUpdate], [_addStoneUpdate, addGoldUpdate]);
            
            _repositoryMock.Setup(library => library.Contains(addGoldUpdate.ItemID)).Returns(true);
            SetupRepositoryContains(true);
            _repositoryMock.Setup(library => library.GetItem(_addStoneUpdate.ItemID)).Returns(_stoneItem);
            _repositoryMock.Setup(library => library.GetItem(addGoldUpdate.ItemID)).Returns(gold);
            
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate, addGoldUpdate]));
            
            VerifyItemFactoryCalls(Times.Never());
            _repositoryMock.Verify(library => library.Contains(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.Contains(addGoldUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.GetItem(_addStoneUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.GetItem(addGoldUpdate.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.AddAmount(_stoneItem.ItemID, _stoneItem.Amount), Times.Once);
            _repositoryMock.Verify(library => library.AddAmount(addGoldUpdate.ItemID, addGoldUpdate.Amount), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
            
            VerifySingleCalls();
            _itemInfoFactoryMock.Verify(library => library.Create(_addStoneUpdate.ItemID, _stoneItem.BaseSellPrice, 1, It.IsAny<Information>()), Times.Once);
            _itemInfoFactoryMock.Verify(library => library.Create(addGoldUpdate.ItemID, gold.BaseSellPrice, 1, It.IsAny<Information>()), Times.Once);
            _itemInfoFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _inventoryMediator.HandleMessages([]));
        }

        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _inventoryMediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_HandleMessage_SummarizerReturnsNothing_Throws()
        {
            InventoryUpdate removeStoneUpdate = _addStoneUpdate with { ActionType = ActionType.REMOVE };
            SetupSummarizerMock([_addStoneUpdate, removeStoneUpdate], []);
            
            Assert.Throws<EmptyCollectionException>(() => _inventoryMediator.HandleMessages([removeStoneUpdate, _addStoneUpdate]));
        }
    }
}