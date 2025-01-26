using IdelPog.Constants;
using IdelPog.Orchestration.Inventory;
using IdelPog.Repository;
using IdelPog.Structures;
using IdelPog.Structures.Item;
using Moq;
using NUnit.Framework;

namespace Tests.Orchestration
{
    [TestFixture]
    public class InventoryMediatorTest
    {
        private IInventoryMediator _inventoryMediator { get; set; }
        private Mock<IInventory> _repositoryMock { get; set; }

        private Item _oakWood { get; set; }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventory>();
            _inventoryMediator = new InventoryMediator(_repositoryMock.Object);
            
            _oakWood = new Item(InventoryID.OAK_WOOD, ItemConstants.OAK_WOOD, 1, 0);

            SetupMocks();
        }

        [TearDown]
        public void Teardown()
        {
            _oakWood = new Item(InventoryID.OAK_WOOD, ItemConstants.OAK_WOOD, 1, 0);
        }

        private void SetupMocks()
        {
        }
      
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Positive_AddItem_DifferentAmounts_AddsItem(int amount)
        {
            ServiceResponse response = _inventoryMediator.AddItem(_oakWood.ID, amount);
            
            Assert.True(response.IsSuccess);
        }
    }
}