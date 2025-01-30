using System;
using IdelPog.Exceptions;
using IdelPog.Orchestration.Inventory;
using IdelPog.Repository;
using IdelPog.Structures;
using IdelPog.Structures.Item;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Orchestration
{
    [TestFixture]
    public class InventoryMediatorTest
    {
        private IInventoryMediator _inventoryMediator { get; set; }
        private Mock<IInventory> _repositoryMock { get; set; }

        private Item _oakWood { get; set; }
        private const int AMOUNT = 10;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventory>();
            _inventoryMediator = new InventoryMediator(_repositoryMock.Object);
            
            _oakWood = ItemFactory.CreateOakWood();

            SetupMocks();
        }

        [TearDown]
        public void Teardown()
        {
            _oakWood = ItemFactory.CreateOakWood();
        }

        private void SetupMocks()
        {
            _repositoryMock.Setup(repo => repo.AddAmount(_oakWood.ID, AMOUNT));
        }
        
        [Test]
        public void Positive_AddAmount_AddsAmount()
        {
            ServiceResponse response = _inventoryMediator.AddAmount(_oakWood.ID, AMOUNT);
            
            Assert.True(response.IsSuccess);
            _repositoryMock.Verify(library => library.AddAmount(_oakWood.ID, AMOUNT));
        }

        [TestCase(InventoryID.WILLOW_WOOD, typeof(NotFoundException))]
        [TestCase(InventoryID.WILLOW_WOOD, typeof(ArgumentException))]
        public void Negative_AddAmount_Catches_Exception(InventoryID inventoryID, Type exception)
        {
            _repositoryMock.Setup(repo => repo.AddAmount(inventoryID, AMOUNT))
                .Throws((Exception) Activator.CreateInstance(exception));
            
            ServiceResponse response = _inventoryMediator.AddAmount(inventoryID, AMOUNT);
            
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Message);
            
            _repositoryMock.Verify(library => library.AddAmount(inventoryID, AMOUNT));
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmount()
        {
            ServiceResponse response = _inventoryMediator.RemoveAmount(_oakWood.ID, AMOUNT);
            
            Assert.True(response.IsSuccess);
            _repositoryMock.Verify(library => library.RemoveAmount(_oakWood.ID, AMOUNT));
        }
        
        [TestCase(InventoryID.WILLOW_WOOD, typeof(NotFoundException))]
        [TestCase(InventoryID.WILLOW_WOOD, typeof(ArgumentException))]
        public void Negative_RemoveAmount_Catches_Exception(InventoryID inventoryID, Type exception)
        {
            _repositoryMock.Setup(repo => repo.RemoveAmount(inventoryID, AMOUNT))
                .Throws((Exception) Activator.CreateInstance(exception));
            
            ServiceResponse response = _inventoryMediator.RemoveAmount(inventoryID, AMOUNT);
            
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Message);
            
            _repositoryMock.Verify(library => library.RemoveAmount(inventoryID, AMOUNT));
        }
    }
}