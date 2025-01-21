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
        private Mock<IRepository<InventoryID, Item>> _repositoryMock { get; set; }

        private Item _oakWood { get; set; }

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<InventoryID, Item>>();
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
            _repositoryMock.Setup(library => library.Get(_oakWood.ID)).Returns(_oakWood);
            _repositoryMock.Setup(library => library.Remove(_oakWood.ID));
        }

        private void VerifyMockCalls(int getCalls, int updateCalls)
        {
            _repositoryMock.Verify(library => library.Get(_oakWood.ID), Times.Exactly(getCalls));
            _repositoryMock.Verify(library => library.Get(_oakWood.ID), Times.Exactly(updateCalls));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Positive_AddItem_DifferentAmounts_AddsItem(int amount)
        {
            ServiceResponse response = _inventoryMediator.AddItem(_oakWood.ID, amount);
            
            Assert.True(response.IsSuccess);
            
            VerifyMockCalls(1, 1);
        }
    }
}