using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests
{
    [TestFixture]
    public class InventoryUpdateMediatorTest
    {
        private IBatchMediator<InventoryUpdate> _inventoryMediator { get; set; }
        private Mock<IInventory> _repositoryMock { get; set; }
        private Mock<IItemFactory> _itemFactoryMock { get; set; }
        private Mock<IInventoryUpdateResponseFactory> _factoryMock { get; set; }
        private Mock<IDispatchMany<InventoryUpdateResponse>> _dispatcherMock { get; set; }

        private InventoryUpdate _inventoryUpdate { get; set; }
        private InventoryUpdateResponse _inventoryUpdateResponse { get; set; }
        private Information _information { get; set; }
        private const int AMOUNT = 9;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventory>();
            _itemFactoryMock = new Mock<IItemFactory>();
            _factoryMock = new Mock<IInventoryUpdateResponseFactory>();
            _dispatcherMock = new Mock<IDispatchMany<InventoryUpdateResponse>>();
            _inventoryMediator = new InventoryUpdateMediator(_repositoryMock.Object, _itemFactoryMock.Object, _factoryMock.Object, _dispatcherMock.Object);
            _information = new Information { Description = "", Name = ""};

            _inventoryUpdate = new InventoryUpdate
            {
                Action = ActionType.ADD,
                ItemID = ItemID.STONE,
                Amount = AMOUNT
            };

            _inventoryUpdateResponse = new InventoryUpdateResponse
            {
                ItemInfo = new ItemInfo
                {
                    Amount = AMOUNT,
                    ItemID = ItemID.STONE,
                    BaseSellPrice = 1
                },
                ActionType = ActionType.ADD,
                MutateType = MutateType.CHANGED
            };

            SetupMocks();
        }

        private void SetupMocks()
        {
            _repositoryMock.Setup(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT));
            _repositoryMock.Setup(library => library.Contains(_inventoryUpdate.ItemID)).Returns(true);

            _factoryMock.Setup(library => library.Create(It.IsAny<Item>(), _inventoryUpdate, MutateType.CHANGED))
                .Returns(_inventoryUpdateResponse);
        }

        [Test]
        public void Positive_AddAmount_AddsAmount()
        {
            _inventoryMediator.HandleMessages([_inventoryUpdate, _inventoryUpdate]);

            _repositoryMock.Verify(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_inventoryUpdate.ItemID));
            _factoryMock.Verify(library => library.Create(It.IsAny<Item>(), _inventoryUpdate, MutateType.CHANGED));
            _dispatcherMock.Verify(library => library.Dispatch(new[] { _inventoryUpdateResponse, _inventoryUpdateResponse }), Times.Once);
            _itemFactoryMock.Verify(library => library.CreateItem(ItemID.STONE, AMOUNT), Times.Never);
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmount()
        {
            InventoryUpdate removeUpdate = new()
            {
                Action = ActionType.REMOVE,
                ItemID = ItemID.STONE,
                Amount = AMOUNT
            };

            _inventoryMediator.HandleMessages([removeUpdate, removeUpdate]);

            _repositoryMock.Verify(library => library.RemoveAmount(_inventoryUpdate.ItemID, AMOUNT));
            _factoryMock.Verify(library => library.Create(It.IsAny<Item>(), removeUpdate, MutateType.CHANGED));
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdateResponse[]>()), Times.Once);
            _itemFactoryMock.Verify(library => library.CreateItem(ItemID.STONE, AMOUNT), Times.Never);
        }

        [Test]
        public void Negative_RemoveAmount_Catches_Exception()
        {
            _repositoryMock.Setup(repo => repo.RemoveAmount(_inventoryUpdate.ItemID, AMOUNT))
                .Throws<Exception>();

            _inventoryMediator.HandleMessages([_inventoryUpdate, _inventoryUpdate]);
        }

        [Test]
        public void Negative_AddAmount_Throws()
        {
            _repositoryMock.Setup(repo => repo.AddAmount(_inventoryUpdate.ItemID, AMOUNT))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _inventoryMediator.HandleMessages([_inventoryUpdate, _inventoryUpdate]));
        }

        [Test]
        public void Positive_AddAmount_NoFoundItem_CreatesItem()
        {
            Item item = ItemFactory.CreateOakWood();
            item.Amount += AMOUNT;

            _repositoryMock.Setup(library => library.Contains(_inventoryUpdate.ItemID)).Returns(false);
            _itemFactoryMock.Setup(library => library.CreateItem(_inventoryUpdate.ItemID, AMOUNT))
                .Returns(item);

            _inventoryMediator.HandleMessages([_inventoryUpdate]);

            _repositoryMock.Verify(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT), Times.Never);
            _repositoryMock.Verify(library => library.Contains(_inventoryUpdate.ItemID));
            _factoryMock.Verify(library => library.Create(It.IsAny<Item>(), _inventoryUpdate, MutateType.CREATED));
            _itemFactoryMock.Verify(library => library.CreateItem(ItemID.STONE, AMOUNT), Times.Once);
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdateResponse[]>()), Times.Once);
        }
    }
}