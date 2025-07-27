using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Inventory;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests
{
    [TestFixture]
    public class InventoryTest
    {
        private IInventory _inventory { get; set; }
        private Mock<IStateRepository<ItemID, Item>> _repositoryMock { get; set; }

        private Item _oakWoodItem { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _oakWoodItem = TestItemFactory.CreateOakWood();
            SetupMock();
        }

        [TearDown]
        public void TearDown()
        {
            _oakWoodItem = TestItemFactory.CreateOakWood();
            SetupMock();
        }

        private void SetupMock()
        {
            _repositoryMock = new Mock<IStateRepository<ItemID, Item>>();
            IHandler throwHandler = new ThrowHandler();

            _inventory = new Inventory(_repositoryMock.Object, new FoundAssertion(throwHandler), new UniqueAssertion(throwHandler));

            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ID)).Returns(_oakWoodItem);
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(true);
        }

        private void ModifyAmountTestRunner(uint amount, ActionType action)
        {
            uint finalAmount = 0;

            switch (action)
            {
                case ActionType.ADD:
                    finalAmount += amount;
                    _inventory.AddAmount(_oakWoodItem.ID, amount);
                    break;
                case ActionType.REMOVE:
                    finalAmount = _oakWoodItem.Amount - amount;
                    _inventory.RemoveAmount(_oakWoodItem.ID, amount);
                    break;
            }

            Assert.That(finalAmount, Is.EqualTo(_oakWoodItem.Amount));

            _repositoryMock.Verify(library => library.Get(_oakWoodItem.ID));

            _repositoryMock.Verify(library => library.Update(_oakWoodItem.ID, _oakWoodItem));
            _repositoryMock.Verify(library => library.Remove(_oakWoodItem.ID), Times.Never());
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(30u)]
        [TestCase(100u)]
        [TestCase(5000u)]
        public void Positive_AddAmount_AddsToItem(uint amount)
        {
            ModifyAmountTestRunner(amount, ActionType.ADD);
        }

        [Test]
        public void Negative_AddAmount_NoItem_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(ItemID.BIRCH_WOOD)).Returns(false);

            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _inventory.AddAmount(ItemID.BIRCH_WOOD, 5));
            Assert.That(exception.Key, Is.EqualTo(ItemID.BIRCH_WOOD));
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(30u)]
        [TestCase(100u)]
        [TestCase(4999u)]
        public void Positive_RemoveAmount_RemovesAmount(uint amount)
        {
            _oakWoodItem.AddAmount(amount + 1);
            ModifyAmountTestRunner(amount, ActionType.REMOVE);
        }

        [Test]
        public void Positive_RemoveAmount_RemovesItem()
        {
            _oakWoodItem.AddAmount(1);
            _inventory.RemoveAmount(_oakWoodItem.ID, 1);

            // The Item will be left with zero amount. Which means, we need to remove it from the Repository
            _repositoryMock.Verify(library => library.Remove(_oakWoodItem.ID));
            // Removing it from the Repository means we shouldn't Update it
            _repositoryMock.Verify(library => library.Update(_oakWoodItem.ID, _oakWoodItem), Times.Never());
        }

        [Test]
        public void Negative_RemoveAmount_NoItemFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(ItemID.BIRCH_WOOD)).Returns(false);

            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _inventory.RemoveAmount(ItemID.BIRCH_WOOD, 1));
            Assert.That(exception.Key, Is.EqualTo(ItemID.BIRCH_WOOD));
        }

        [Test]
        public void Positive_AddItem_AddsItemToRepository()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(false);

            _inventory.AddItem(_oakWoodItem);

            _repositoryMock.Verify(library => library.Add(_oakWoodItem.ID, It.IsAny<Item>()));
        }

        [Test]
        public void Positive_AddItem_ItemHasCorrectAmount()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(false);

            const int amount = 1;
            _oakWoodItem.AddAmount(amount);


            _repositoryMock.Setup(library => library.Add(_oakWoodItem.ID, It.IsAny<Item>()))
                .Callback<ItemID, Item>((id, item) =>
                {
                    Assert.That(_oakWoodItem.ID, Is.EqualTo(id));
                    Assert.That(amount, Is.EqualTo(item.Amount));
                });

            _inventory.AddItem(_oakWoodItem);
        }

        [Test]
        public void Negative_AddItem_ItemExists_Throws()
        {
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _inventory.AddItem(_oakWoodItem));
            Assert.That(exception.ID, Is.EqualTo(_oakWoodItem.ID));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            bool contains = _inventory.Contains(_oakWoodItem.ID);
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Negative_Contains_ReturnsFalse()
        {
            bool contains = _inventory.Contains(ItemID.WILLOW_WOOD);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Positive_GetItem_ReturnsItem()
        {
            Item item = _inventory.GetItem(_oakWoodItem.ID);

            Assert.That(item, Is.Not.Null);
            Assert.That(item, Is.EqualTo(_oakWoodItem));
        }

        [Test]
        public void Negative_GetItem_NoItemFound_Throws()
        {
            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _inventory.GetItem(ItemID.WILLOW_WOOD));
            Assert.That(exception.Key, Is.EqualTo(ItemID.WILLOW_WOOD));
        }
    }
}