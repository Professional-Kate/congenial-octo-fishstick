using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Exceptions;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests
{
    [TestFixture]
    public sealed class InventoryTest
    {
        private IInventory _inventory { get; set; }
        private Mock<IStateRepository<ItemID, Item>> _repositoryMock { get; set; }

        private Item _oakWoodItem { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _oakWoodItem = ItemFactory.CreateOakWood();
        }

        [SetUp]
        public void Setup()
        {
            _oakWoodItem = ItemFactory.CreateOakWood();
            _repositoryMock = new Mock<IStateRepository<ItemID, Item>>();

            _inventory = new Inventory.Service.Inventory(_repositoryMock.Object, new FoundAssertion(), new UniqueAssertion(), new AmountAssertion());
        }

        private void VerifyRepositoryUpdate()
        {
            _repositoryMock.Verify(library => library.Contains(_oakWoodItem.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.Get(_oakWoodItem.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.Update(_oakWoodItem.ItemID, _oakWoodItem), Times.Once);
            _repositoryMock.Verify(library => library.Remove(_oakWoodItem.ItemID), Times.Never());
            _repositoryMock.Verify(library => library.Add(_oakWoodItem.ItemID, _oakWoodItem), Times.Never());
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(30u)]
        [TestCase(100u)]
        [TestCase(5000u)]
        public void Positive_AddAmount_AddsAmountToItem(uint amount)
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ItemID)).Returns(_oakWoodItem);
            
            _inventory.AddAmount(_oakWoodItem.ItemID, amount);
            
            VerifyRepositoryUpdate();
        }

        [Test]
        public void Negative_AddAmount_NoItem_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(ItemID.IRON)).Returns(false);

            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _inventory.AddAmount(ItemID.IRON, 5));
            Assert.That(exception.Key, Is.EqualTo(ItemID.IRON));
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(30u)]
        [TestCase(100u)]
        [TestCase(4999u)]
        public void Positive_RemoveAmount_RemovesAmount(uint amount)
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ItemID)).Returns(_oakWoodItem);

            _oakWoodItem.Amount = amount + 1;
            _inventory.RemoveAmount(_oakWoodItem.ItemID, amount);
            
            VerifyRepositoryUpdate();
        }

        [Test]
        public void Positive_RemoveAmount_RemovesItem()
        {
            Item singleAmountItem = new(ItemID.STONE, 0, new Information() { Description = "", Name = ""}, 1);
            
            _repositoryMock.Setup(library => library.Contains(singleAmountItem.ItemID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(singleAmountItem.ItemID)).Returns(singleAmountItem);
            
            _inventory.RemoveAmount(singleAmountItem.ItemID, 1);

            // The Item will be left with zero amount. Which means, we need to remove it from the Repository
            _repositoryMock.Verify(library => library.Remove(singleAmountItem.ItemID), Times.Once);
            _repositoryMock.Verify(library => library.Update(singleAmountItem.ItemID, singleAmountItem), Times.Never());
        }

        [Test]
        public void Negative_RemoveAmount_ItemHasLessAmount_Throws()
        {
            Item singleAmountItem = new(ItemID.STONE, 0, new Information { Description = "", Name = ""}, 1);
            
            _repositoryMock.Setup(library => library.Contains(singleAmountItem.ItemID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(singleAmountItem.ItemID)).Returns(singleAmountItem);
            
            Assert.Throws<InsufficientAmountException>(() => _inventory.RemoveAmount(singleAmountItem.ItemID, 2));

            // The Item will be left with zero amount. Which means, we need to remove it from the Repository
            _repositoryMock.Verify(library => library.Remove(singleAmountItem.ItemID), Times.Never());
            _repositoryMock.Verify(library => library.Update(singleAmountItem.ItemID, singleAmountItem), Times.Never());
        }

        [Test]
        public void Negative_RemoveAmount_NoItemFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(ItemID.IRON)).Returns(false);

            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _inventory.RemoveAmount(ItemID.IRON, 1));
            Assert.That(exception.Key, Is.EqualTo(ItemID.IRON));
        }

        [Test]
        public void Positive_AddItem_AddsItemToRepository()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(false);

            _inventory.AddItem(_oakWoodItem);

            _repositoryMock.Verify(library => library.Add(_oakWoodItem.ItemID, _oakWoodItem));
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
                    Assert.That(item.Amount, Is.EqualTo(amount));
                });

            _inventory.AddItem(_oakWoodItem);
        }

        [Test]
        public void Negative_AddItem_ItemExists_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(true);
            
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _inventory.AddItem(_oakWoodItem));
            Assert.That(exception.ID, Is.EqualTo(_oakWoodItem.ItemID));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(true);
            
            bool contains = _inventory.Contains(_oakWoodItem.ItemID);
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Negative_Contains_ReturnsFalse()
        {
            bool contains = _inventory.Contains(ItemID.COPPER);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Positive_GetItem_ReturnsItem()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ItemID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ItemID)).Returns(_oakWoodItem);
            
            Item item = _inventory.GetItem(_oakWoodItem.ItemID);

            Assert.That(item, Is.Not.Null);
            Assert.That(item, Is.EqualTo(_oakWoodItem));
        }

        [Test]
        public void Negative_GetItem_NoItemFound_Throws()
        {
            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _inventory.GetItem(ItemID.COPPER));
            Assert.That(exception.Key, Is.EqualTo(ItemID.COPPER));
        }
    }
}