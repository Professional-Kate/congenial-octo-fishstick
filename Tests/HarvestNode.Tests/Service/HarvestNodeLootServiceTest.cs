using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Service.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Service
{
    [TestFixture]
    public sealed class HarvestNodeLootServiceTest
    {
        private IHarvestNodeLootService _nodeLootService;
        private Mock<ILootService<ResourceID>> _itemServiceMock;
        private Mock<ILootService<LocationID>> _locationServiceMock;
        
        private Contracts.HarvestNode _harvestNode;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _harvestNode = new Contracts.HarvestNode
            {
                LocationID = LocationID.FOREST,
                ResourceID = ResourceID.LEAF_LITTER,
                Information = new Information { Description = "", Name = "" },
                Levelable = new Levelable(0, 0, 0, 0)
            };

            _itemServiceMock = new Mock<ILootService<ResourceID>>();
            _locationServiceMock = new Mock<ILootService<LocationID>>();
            
            _nodeLootService = new HarvestNodeLootService(_itemServiceMock.Object, _locationServiceMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _itemServiceMock.Reset();
            _locationServiceMock.Reset();
        }

        private void SetupItemService(bool shouldGrant)
        {
            _itemServiceMock.Setup(library => library.ShouldGrant(It.IsAny<ResourceID>())).Returns(shouldGrant);
        }
        
        private void SetupLocationService(bool shouldGrant)
        {
            _locationServiceMock.Setup(library => library.ShouldGrant(It.IsAny<LocationID>())).Returns(shouldGrant);
        }

        private static void AssertUpdateLength(int length, IReadOnlyList<InventoryUpdate> updates)
        {
            Assert.That(updates, Has.Count.EqualTo(length));
        }
        
        [Test]
        public void Positive_GenerateInventoryUpdates_LootServiceReturnsFalse_NoUpdates()
        {
            SetupItemService(false);
            SetupLocationService(false);
            
            IReadOnlyList<InventoryUpdate> inventoryUpdates = _nodeLootService.GenerateInventoryUpdates(_harvestNode);
            
            AssertUpdateLength(0, inventoryUpdates);
        }

        [Test]
        public void Positive_GenerateInventoryUpdate_BothServicesReturnTrue_ReturnsTwoUpdates()
        {
            SetupItemService(true);
            SetupLocationService(true);
            
            IReadOnlyList<InventoryUpdate> inventoryUpdates = _nodeLootService.GenerateInventoryUpdates(_harvestNode);
            
            AssertUpdateLength(2, inventoryUpdates);
        }
        
        [Test]
        public void Positive_GenerateInventoryUpdates_LocationService_ThrowsNotFound_Catches()
        {
            SetupItemService(true);
            _locationServiceMock.Setup(library => library.GenerateItemID(_harvestNode.LocationID)).Throws(new NotFoundException<LocationID>(_harvestNode.LocationID));
            
            IReadOnlyList<InventoryUpdate> inventoryUpdates = _nodeLootService.GenerateInventoryUpdates(_harvestNode);
            
            AssertUpdateLength(1, inventoryUpdates);
        }
        
        [Test]
        public void Positive_GenerateInventoryUpdates_ItemService_ThrowsNotFound_Catches()
        {
            _itemServiceMock.Setup(library => library.GenerateItemID(_harvestNode.ResourceID)).Throws(new NotFoundException<ResourceID>(_harvestNode.ResourceID));
            SetupLocationService(true);

            IReadOnlyList<InventoryUpdate> inventoryUpdates = _nodeLootService.GenerateInventoryUpdates(_harvestNode);
            
            AssertUpdateLength(1, inventoryUpdates);
        }

        [Test]
        public void Positive_GenerateInventoryUpdates_ShouldGrant_ThrowsNotFound_Catches()
        {
            _itemServiceMock.Setup(library => library.ShouldGrant(_harvestNode.ResourceID)).Throws(new NotFoundException<ResourceID>(_harvestNode.ResourceID));
            SetupLocationService(true);

            IReadOnlyList<InventoryUpdate> inventoryUpdates = _nodeLootService.GenerateInventoryUpdates(_harvestNode);
            
            AssertUpdateLength(1, inventoryUpdates);
        }
    }
}