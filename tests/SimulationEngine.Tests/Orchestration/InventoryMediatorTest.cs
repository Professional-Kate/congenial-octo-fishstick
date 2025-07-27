using IdelPog.Common.Enums;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Structures;
using IdelPog.SimulationEngine.Structures.Types;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class InventoryMediatorTest
    {
        private IInventoryMediator _inventoryMediator { get; set; }
        private Mock<IInventory> _repositoryMock { get; set; }
        private Mock<IItemFactory> _itemFactoryMock { get; set; }
        private Mock<IInventoryUpdateDTOFactory> _factoryMock { get; set; }
        private Mock<IDispatchMany<InventoryUpdateDTO>> _dispatcherMock { get; set; }

        private InventoryUpdate _inventoryUpdate { get; set; }
        private InventoryUpdateDTO _inventoryUpdateDTO { get; set; }
        private Information _information { get; set; }
        private const int AMOUNT = 9;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventory>();
            _itemFactoryMock = new Mock<IItemFactory>();
            _factoryMock = new Mock<IInventoryUpdateDTOFactory>();
            _dispatcherMock = new Mock<IDispatchMany<InventoryUpdateDTO>>();
            _inventoryMediator = new InventoryMediator(_repositoryMock.Object, _itemFactoryMock.Object, _factoryMock.Object, _dispatcherMock.Object);
            _information = new Information { Description = "", Name = ""};

            _inventoryUpdate = new InventoryUpdate
            {
                Action = ActionType.ADD,
                ItemID = ItemID.OAK_WOOD,
                Amount = AMOUNT
            };

            _inventoryUpdateDTO = new InventoryUpdateDTO
            {
                ItemDTO = new ItemDTO
                {
                    Amount = AMOUNT,
                    ItemID = ItemID.OAK_WOOD,
                    SellPrice = 1
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

            _factoryMock.Setup(library => library.CreateInventoryUpdateDTO(It.IsAny<Item>(), _inventoryUpdate, MutateType.CHANGED))
                .Returns(_inventoryUpdateDTO);
        }

        [Test]
        public void Positive_AddAmount_AddsAmount()
        {
            _inventoryMediator.UpdateInventory([_inventoryUpdate, _inventoryUpdate]);

            _repositoryMock.Verify(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_inventoryUpdate.ItemID));
            _factoryMock.Verify(library => library.CreateInventoryUpdateDTO(It.IsAny<Item>(), _inventoryUpdate, MutateType.CHANGED));
            _dispatcherMock.Verify(library => library.Dispatch(new[] { _inventoryUpdateDTO, _inventoryUpdateDTO }), Times.Once);
            _itemFactoryMock.Verify(library => library.CreateItem(ItemID.OAK_WOOD, AMOUNT), Times.Never);
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmount()
        {
            InventoryUpdate removeUpdate = new()
            {
                Action = ActionType.REMOVE,
                ItemID = ItemID.OAK_WOOD,
                Amount = AMOUNT
            };

            _inventoryMediator.UpdateInventory([removeUpdate, removeUpdate]);

            _repositoryMock.Verify(library => library.RemoveAmount(_inventoryUpdate.ItemID, AMOUNT));
            _factoryMock.Verify(library => library.CreateInventoryUpdateDTO(It.IsAny<Item>(), removeUpdate, MutateType.CHANGED));
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdateDTO[]>()), Times.Once);
            _itemFactoryMock.Verify(library => library.CreateItem(ItemID.OAK_WOOD, AMOUNT), Times.Never);
        }

        [Test]
        public void Negative_RemoveAmount_Catches_Exception()
        {
            _repositoryMock.Setup(repo => repo.RemoveAmount(_inventoryUpdate.ItemID, AMOUNT))
                .Throws<Exception>();

            _inventoryMediator.UpdateInventory([_inventoryUpdate, _inventoryUpdate]);
        }

        [Test]
        public void Negative_AddAmount_Throws()
        {
            _repositoryMock.Setup(repo => repo.AddAmount(_inventoryUpdate.ItemID, AMOUNT))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _inventoryMediator.UpdateInventory([_inventoryUpdate, _inventoryUpdate]));
        }

        [Test]
        public void Positive_AddAmount_NoFoundItem_CreatesItem()
        {
            Item item = TestItemFactory.CreateOakWood();
            item.AddAmount(AMOUNT);

            _repositoryMock.Setup(library => library.Contains(_inventoryUpdate.ItemID)).Returns(false);
            _itemFactoryMock.Setup(library => library.CreateItem(_inventoryUpdate.ItemID, AMOUNT))
                .Returns(item);

            _inventoryMediator.UpdateInventory([_inventoryUpdate]);

            _repositoryMock.Verify(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT), Times.Never);
            _repositoryMock.Verify(library => library.Contains(_inventoryUpdate.ItemID));
            _factoryMock.Verify(library => library.CreateInventoryUpdateDTO(It.IsAny<Item>(), _inventoryUpdate, MutateType.CREATED));
            _itemFactoryMock.Verify(library => library.CreateItem(ItemID.OAK_WOOD, AMOUNT), Times.Once);
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdateDTO[]>()), Times.Once);
        }
    }
}