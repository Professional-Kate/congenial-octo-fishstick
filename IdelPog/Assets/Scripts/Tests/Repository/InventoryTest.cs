using System;
using IdelPog.Exceptions;
using IdelPog.Repository;
using IdelPog.Repository.Inventory;
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

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(30)]
        [TestCase(100)]
        [TestCase(5000)]
        public void Positive_AddAmount_AddsToItem(int amount)
        {
             _inventory.AddAmount(_oakWoodItem.ID, amount);
             
             Assert.AreEqual(amount, _oakWoodItem.Amount);
             
             _repositoryMock.Verify(library => library.Get(_oakWoodItem.ID));
             _repositoryMock.Verify(library => library.Update(_oakWoodItem.ID, _oakWoodItem));
        }

        [Test]
        public void Negative_AddAmount_NoType_Throws()
        {
            _repositoryMock.Setup(library => library.Get(InventoryID.NO_TYPE))
                .Throws<NoTypeException>();
            

            Assert.Throws<NoTypeException>(() => _inventory.AddAmount(InventoryID.NO_TYPE, 1));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void Negative_AddAmount_BadAmount_Throws(int amount)
        {
            Assert.Throws<ArgumentException>(() => _inventory.AddAmount(_oakWoodItem.ID, amount));
        }
        
        // TODO : Add tests for the repository class throwing exceptions, we want this service class to do nothing with the exceptions to allow the mediator to handle it
    }
}