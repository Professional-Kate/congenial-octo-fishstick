using IdelPog.SimulationEngine.Flows.Inventory;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures;
using IdelPog.SimulationEngine.Structures.Types;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class InventoryMediatorTest
    {
        private IInventoryMediator _inventoryMediator { get; set; }
        private Mock<IInventory> _repositoryMock { get; set; }
        private Mock<IMapper<ItemID>> _mapperMock { get; set; }

        private InventoryUpdate _inventoryUpdate { get; set; }
        private Information _information { get; set; }
        private const int AMOUNT = 9;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventory>();
            _mapperMock = new Mock<IMapper<ItemID>>();
            _inventoryMediator = new InventoryMediator(_repositoryMock.Object, _mapperMock.Object);
            _information = new Information("", "");

            _inventoryUpdate = new InventoryUpdate()
            {
                Action = ActionType.ADD,
                ItemID = ItemID.OAK_WOOD,
                Amount = AMOUNT
            };
            
            SetupMocks();
        }

        private void SetupMocks()
        {
            _repositoryMock.Setup(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT));
            _repositoryMock.Setup(library => library.Contains(_inventoryUpdate.ItemID)).Returns(true);
        }
        
        [Test]
        public void Positive_AddAmount_AddsAmount()
        {
            _inventoryMediator.UpdateInventory([_inventoryUpdate]);
            
            _repositoryMock.Verify(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_inventoryUpdate.ItemID));
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
            
            _inventoryMediator.UpdateInventory([removeUpdate]);
            
            _repositoryMock.Verify(library => library.RemoveAmount(_inventoryUpdate.ItemID, AMOUNT));
        }
        
        [Test]
        public void Negative_RemoveAmount_Catches_Exception()
        {
            _repositoryMock.Setup(repo => repo.RemoveAmount(_inventoryUpdate.ItemID, AMOUNT))
                .Throws<Exception>();
            
            _inventoryMediator.UpdateInventory([_inventoryUpdate]);
        }
        
        [Test]
        public void Negative_AddAmount_Catches_Exception()
        {
            _repositoryMock.Setup(repo => repo.AddAmount(_inventoryUpdate.ItemID, AMOUNT))
                .Throws<Exception>();
            
            _inventoryMediator.UpdateInventory([_inventoryUpdate]);
        }

        [Test]
        public void Positive_AddAmount_NoFoundItem_CreatesItem()
        {
            _repositoryMock.Setup(library => library.Contains(_inventoryUpdate.ItemID)).Returns(false);
            _mapperMock.Setup(library => library.GetInformation(_inventoryUpdate.ItemID)).Returns(_information);
            
            _inventoryMediator.UpdateInventory([_inventoryUpdate]);
            
            _repositoryMock.Verify(library => library.AddAmount(_inventoryUpdate.ItemID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_inventoryUpdate.ItemID));
            _mapperMock.Verify(library => library.GetInformation(_inventoryUpdate.ItemID));
        }
    }
}