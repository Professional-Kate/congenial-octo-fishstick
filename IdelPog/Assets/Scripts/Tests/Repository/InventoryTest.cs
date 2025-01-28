using System;
using IdelPog.Exceptions;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Item;
using Moq;
using NUnit.Framework;
using ItemFactory = Tests.Utils.ItemFactory;

namespace Tests.Repository
{
    [TestFixture]
    public class InventoryTest
    {
        private IInventory _inventory { get; set; }
        private Mock<IRepository<InventoryID, Item>> _repositoryMock { get; set; }
        private Mock<IItemFactory> _itemFactoryMock { get; set; }

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
            _repositoryMock = new Mock<IRepository<InventoryID, Item>>();
            _itemFactoryMock = new Mock<IItemFactory>();
            _inventory = new Inventory(_repositoryMock.Object, _itemFactoryMock.Object);
            
            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ID)).Returns(_oakWoodItem);
            _repositoryMock.Setup(library => library.Get(InventoryID.BIRCH_WOOD)).Throws<NotFoundException>();
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(true);
            
            _itemFactoryMock.Setup(library => library.CreateItem(It.IsAny<InventoryID>(), It.IsAny<int>())).Returns(_oakWoodItem);
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
            
            Assert.AreEqual(finalAmount, _oakWoodItem.Amount);
             
            _repositoryMock.Verify(library => library.Get(_oakWoodItem.ID));
            
            _repositoryMock.Verify(library => library.Update(_oakWoodItem.ID, _oakWoodItem));
            _repositoryMock.Verify(library => library.Remove(_oakWoodItem.ID), Times.Never());
            _repositoryMock.Verify(library => library.Contains(_oakWoodItem.ID));
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
        public void Positive_AddAmount_AddsItem()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(false);
            
            _inventory.AddAmount(_oakWoodItem.ID, 5);
            
            _repositoryMock.Verify(library => library.Add(_oakWoodItem.ID, _oakWoodItem));
            _repositoryMock.Verify(library => library.Get(_oakWoodItem.ID), Times.Never());
            Assert.AreEqual(5, _oakWoodItem.Amount);
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void Negative_AddAmount_BadAmount_Throws(int amount)
        {
            Assert.Throws<ArgumentException>(() => _inventory.AddAmount(_oakWoodItem.ID, amount));
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
            _repositoryMock.Setup(library => library.Contains(InventoryID.BIRCH_WOOD)).Returns(false);

            Assert.Throws<NotFoundException>(() => _inventory.RemoveAmount(InventoryID.BIRCH_WOOD, 1));
        }

        [Test]
        public void Negative_RemoveAmount_AmountUnderZero_Throws()
        {
            Assert.Throws<ArgumentException>(() => _inventory.RemoveAmount(_oakWoodItem.ID, 10));
        }
        
        [TestCase(0)]
        [TestCase(-10)]
        public void Negative_RemoveAmount_BadAmount_Throws(int amount)
        {
            Assert.Throws<ArgumentException>(() => _inventory.RemoveAmount(_oakWoodItem.ID, amount));
        }

        [Test]
        public void Positive_AddItem_AddsItemToRepository()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(false);
            
            _inventory.AddItem(_oakWoodItem.ID, 1);

            _repositoryMock.Verify(library => library.Add(_oakWoodItem.ID, It.IsAny<Item>()));
            _repositoryMock.Verify(library => library.Contains(_oakWoodItem.ID));
        }

        [Test]
        public void Positive_AddItem_ItemHasCorrectAmount()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWoodItem.ID)).Returns(false);

            const int amount = 1;
            _oakWoodItem.AddAmount(amount);

            _itemFactoryMock.Setup(library => library.CreateItem(_oakWoodItem.ID, amount)).Returns(_oakWoodItem);
            
            _repositoryMock.Setup(library => library.Add(_oakWoodItem.ID, It.IsAny<Item>()))
                .Callback<InventoryID, Item>(((id, item) =>
                {
                    Assert.AreEqual(_oakWoodItem.ID, id);
                    Assert.AreEqual(amount, item.Amount);
                }));
            
            _inventory.AddItem(_oakWoodItem.ID, amount);
            _itemFactoryMock.Verify(library => library.CreateItem(_oakWoodItem.ID, amount));
            _repositoryMock.Verify(library => library.Contains(_oakWoodItem.ID));
        }

        [Test]
        public void Negative_AddItem_ItemExists_Throws()
        {
            _inventory.AddItem(InventoryID.WILLOW_WOOD, 1);
            
            _repositoryMock.Setup(library => library.Contains(InventoryID.WILLOW_WOOD)).Returns(true);
            
            Assert.Throws<ArgumentException>(() => _inventory.AddItem(InventoryID.WILLOW_WOOD, 10));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void Negative_AddItem_BadAmount_Throws(int amount)
        {
            Assert.Throws<ArgumentException>(() => _inventory.AddItem(_oakWoodItem.ID, amount));
        }
    }
}