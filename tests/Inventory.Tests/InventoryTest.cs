using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Service;
using Moq;

namespace Inventory.Tests
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
            _oakWoodItem = ItemFactory.CreateOakWood();
            SetupMock();
        }

        [TearDown]
        public void TearDown()
        {
            _oakWoodItem = ItemFactory.CreateOakWood();
            SetupMock();
        }

        private void SetupMock()
        {
            _repositoryMock = new Mock<IStateRepository<ItemID, Item>>();
            IHandler throwHandler = new ThrowHandler();

            _inventory = new IdelPog.Inventory.Service.Inventory(_repositoryMock.Object, new FoundAssertion(throwHandler), new UniqueAssertion(throwHandler));

            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ItemID)).Returns(_oakWoodItem);
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(true);
        }

        private void ModifyAmountTestRunner(uint amount, ActionType action)
        {
            uint finalAmount = 0;

            switch (action)
            {
                case ActionType.ADD:
                    finalAmount += amount;
                    _inventory.AddAmount(_oakWoodItem.ItemID, amount);
                    break;
                case ActionType.REMOVE:
                    finalAmount = _oakWoodItem.Amount - amount;
                    _inventory.RemoveAmount(_oakWoodItem.ItemID, amount);
                    break;
            }

            Assert.That(finalAmount, Is.EqualTo(_oakWoodItem.Amount));

            _repositoryMock.Verify(library => library.Get(_oakWoodItem.ItemID));

            _repositoryMock.Verify(library => library.Update(_oakWoodItem.ItemID, _oakWoodItem));
            _repositoryMock.Verify(library => library.Remove(_oakWoodItem.ItemID), Times.Never());
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
            _oakWoodItem.Amount += amount + 1;
            ModifyAmountTestRunner(amount, ActionType.REMOVE);
        }

        [Test]
        public void Positive_RemoveAmount_RemovesItem()
        {
            _oakWoodItem.Amount++;
            _inventory.RemoveAmount(_oakWoodItem.ItemID, 1);

            // The Item will be left with zero amount. Which means, we need to remove it from the Repository
            _repositoryMock.Verify(library => library.Remove(_oakWoodItem.ItemID));
            // Removing it from the Repository means we shouldn't Update it
            _repositoryMock.Verify(library => library.Update(_oakWoodItem.ItemID, _oakWoodItem), Times.Never());
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
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(false);

            _inventory.AddItem(_oakWoodItem);

            _repositoryMock.Verify(library => library.Add(_oakWoodItem.ItemID, It.IsAny<Item>()));
        }

        [Test]
        public void Positive_AddItem_ItemHasCorrectAmount()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(false);

            const int amount = 1;
            _oakWoodItem.Amount += amount;


            _repositoryMock.Setup(library => library.Add(_oakWoodItem.ItemID, It.IsAny<Item>()))
                .Callback<ItemID, Item>((id, item) =>
                {
                    Assert.That(_oakWoodItem.ItemID, Is.EqualTo(id));
                    Assert.That(amount, Is.EqualTo(item.Amount));
                });

            _inventory.AddItem(_oakWoodItem);
        }

        [Test]
        public void Negative_AddItem_ItemExists_Throws()
        {
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _inventory.AddItem(_oakWoodItem));
            Assert.That(exception.ID, Is.EqualTo(_oakWoodItem.ItemID));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            bool contains = _inventory.Contains(_oakWoodItem.ItemID);
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
            Item item = _inventory.GetItem(_oakWoodItem.ItemID);

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