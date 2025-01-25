using System;
using IdelPog.Repository;
using IdelPog.Repository.Inventory;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Item;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Repository
{
    [TestFixture]
    public class InventoryTest
    {
        private IInventory _inventory { get; set; }
        private Mock<IRepository<InventoryID, Item>> _repositoryMock { get; set; }

        private Item _oakWoodItem { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repositoryMock = new Mock<IRepository<InventoryID, Item>>();
            _inventory = new Inventory(_repositoryMock.Object);
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
            _repositoryMock.Setup(library => library.Get(_oakWoodItem.ID)).Returns(_oakWoodItem);
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
        
        [TestCase(0)]
        [TestCase(-10)]
        public void Negative_RemoveAmount_BadAmount_Throws(int amount)
        {
            Assert.Throws<ArgumentException>(() => _inventory.RemoveAmount(_oakWoodItem.ID, amount));
        }
    }
}