using System;
using IdelPog.Exceptions;
using IdelPog.Orchestration;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Models.Item;
using Moq;
using NUnit.Framework;
using UnityEngine;
using ItemFactory = Tests.Utils.ItemFactory;

namespace Tests.Orchestration
{
    [TestFixture]
    public class InventoryMediatorTest
    {
        private IInventoryMediator _inventoryMediator { get; set; }
        private Mock<IInventory> _repositoryMock { get; set; }
        private Mock<IItemFactory> _itemFactoryMock { get; set; }
        private Mock<IMapper<InventoryID>> _mapperMock { get; set; }

        private Item _oakWood { get; set; }
        private Information _information { get; set; }
        private const int AMOUNT = 9;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventory>();
            _itemFactoryMock = new Mock<IItemFactory>();
            _mapperMock = new Mock<IMapper<InventoryID>>();
            _inventoryMediator = new InventoryMediator(_repositoryMock.Object, _itemFactoryMock.Object, _mapperMock.Object);
            _information = Information.Create("", "");
            
            _oakWood = ItemFactory.CreateOakWood();
            _oakWood.AddAmount(1);

            SetupMocks();
        }

        [TearDown]
        public void Teardown()
        {
            _oakWood = ItemFactory.CreateOakWood();
            _oakWood.AddAmount(1);
        }

        private void SetupMocks()
        {
            _repositoryMock.Setup(library => library.AddAmount(_oakWood.ID, AMOUNT));
            _repositoryMock.Setup(library => library.Contains(_oakWood.ID)).Returns(true);
        }
        
        [Test]
        public void Positive_AddAmount_AddsAmount()
        {
            ServiceResponse response = _inventoryMediator.AddAmount(_oakWood.ID, AMOUNT);
            
            Assert.True(response.IsSuccess);
            _repositoryMock.Verify(library => library.AddAmount(_oakWood.ID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_oakWood.ID));
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
            
            Debug.Log(response.Message);
            
            Assert.True(response.IsSuccess);
            _repositoryMock.Verify(library => library.RemoveAmount(_oakWood.ID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_oakWood.ID));
        }
        
        [TestCase(InventoryID.WILLOW_WOOD, typeof(NotFoundException))]
        [TestCase(InventoryID.OAK_WOOD, typeof(ArgumentException))]
        public void Negative_RemoveAmount_Catches_Exception(InventoryID inventoryID, Type exception)
        {
            _repositoryMock.Setup(repo => repo.RemoveAmount(inventoryID, AMOUNT))
                .Throws((Exception) Activator.CreateInstance(exception));
            
            ServiceResponse response = _inventoryMediator.RemoveAmount(inventoryID, AMOUNT);
            
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Message);
        }

        [Test]
        public void Positive_AddAmount_NoFoundItem_CreatesItem()
        {
            _repositoryMock.Setup(library => library.Contains(_oakWood.ID)).Returns(false);
            _itemFactoryMock.Setup(library => library.CreateItem(_oakWood.ID, _information, 1, AMOUNT)).Returns(_oakWood);
            _mapperMock.Setup(library => library.GetInformation(_oakWood.ID)).Returns(_information);
            
            ServiceResponse response = _inventoryMediator.AddAmount(_oakWood.ID, AMOUNT);
            
            Assert.True(response.IsSuccess);
            
            _repositoryMock.Verify(library => library.AddAmount(_oakWood.ID, AMOUNT));
            _repositoryMock.Verify(library => library.Contains(_oakWood.ID));
            _itemFactoryMock.Verify(library => library.CreateItem(_oakWood.ID, _information, 1, AMOUNT));
            _mapperMock.Verify(library => library.GetInformation(_oakWood.ID));
        }
    }
}