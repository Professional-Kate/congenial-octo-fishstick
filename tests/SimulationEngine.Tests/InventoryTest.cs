using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Structures.Types;
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

            _inventory = new Inventory(_repositoryMock.Object, new FoundAssertion(throwHandler), new NumberAssertion(throwHandler),
                new UniqueAssertion(throwHandler));

            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ID)).Returns(_oakWoodItem);
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(true);

            _repositoryMock.Setup(library => library.Get(ItemID.BIRCH_WOOD))
                .Throws(new NotFoundException(ItemID.BIRCH_WOOD));
        }

        private void ModifyAmountTestRunner(int amount, ActionType action)
        {
            int finalAmount = 0;

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

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(30)]
        [TestCase(100)]
        [TestCase(5000)]
        public void Positive_AddAmount_AddsToItem(int amount)
        {
            ModifyAmountTestRunner(amount, ActionType.ADD);
        }

        [Test]
        public void Negative_AddAmount_NoItem_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(ItemID.BIRCH_WOOD)).Returns(false);

            Assert.Throws<NotFoundException>(() => _inventory.AddAmount(ItemID.BIRCH_WOOD, 5));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_AddAmount_NegativeAmount_Throws(int amount)
        {
            NegativeNumberException negativeNumberException = Assert.Throws<NegativeNumberException>(() => _inventory.AddAmount(_oakWoodItem.ID, amount));
            Assert.That(negativeNumberException.Number, Is.EqualTo(amount));
        }


        [TestCase(1)]
        [TestCase(10)]
        [TestCase(30)]
        [TestCase(100)]
        [TestCase(4999)]
        public void Positive_RemoveAmount_RemovesAmount(int amount)
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

            Assert.Throws<NotFoundException>(() => _inventory.RemoveAmount(ItemID.BIRCH_WOOD, 1));
        }

        [Test]
        public void Negative_RemoveAmount_AmountUnderZero_Throws()
        {
            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() => _inventory.RemoveAmount(_oakWoodItem.ID, 10));
            Assert.That(exception.Number, Is.EqualTo(-10));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_RemoveAmount_NegativeAmount_Throws(int amount)
        {
            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() => _inventory.RemoveAmount(_oakWoodItem.ID, amount));
            Assert.That(exception.Number, Is.EqualTo(amount));
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
            Assert.Throws<DuplicateItemException>(() => _inventory.AddItem(_oakWoodItem));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_AddItem_NegativeAmount_Throws(int amount)
        {
            Item itemWithBadAmount = new(ItemID.WILLOW_WOOD, new Information("", ""), 1, amount);

            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() => _inventory.AddItem(itemWithBadAmount));
            Assert.That(exception.Number, Is.EqualTo(amount));
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
            Assert.Throws<NotFoundException>(() => _inventory.GetItem(ItemID.WILLOW_WOOD));
        }
    }
}