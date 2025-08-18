using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Loot.Contracts;
using IdelPog.Loot.Service;
using IdelPog.Loot.Service.Interface;
using Moq;

namespace Loot.Tests
{
    [TestFixture]
    public class LootServiceTest
    {
        private ILootService _lootService;
        private Mock<IAssetRepository<ItemID, ILootTable>> _lootTableRepositoryMock;
        private Mock<IDispatchOne<InventoryUpdate>> _inventoryUpdateDispatcherMock;
        private Mock<ILootTable> _weightedLootTableMock;

        private const ItemID ITEM_ID = ItemID.STONE;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootTableRepositoryMock = new Mock<IAssetRepository<ItemID, ILootTable>>();
            _inventoryUpdateDispatcherMock = new Mock<IDispatchOne<InventoryUpdate>>();
            _weightedLootTableMock = new Mock<ILootTable>();
            
            _lootService = new LootService(_lootTableRepositoryMock.Object, _inventoryUpdateDispatcherMock.Object, new FoundAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _lootTableRepositoryMock.Reset();
        }

        [Test]
        public void Positive_DispatchInventoryUpdates_DispatchesExpectedUpdate()
        {
            _lootTableRepositoryMock.Setup(library => library.Contains(ITEM_ID)).Returns(true);
            _lootTableRepositoryMock.Setup(library => library.Get(ITEM_ID)).Returns(_weightedLootTableMock.Object);
            
            Assert.DoesNotThrow(() => _lootService.DispatchInventoryUpdates(ITEM_ID));
            
            _weightedLootTableMock.Verify(library => library.Roll(), Times.Once);
            
            _lootTableRepositoryMock.Verify(library => library.Contains(ITEM_ID), Times.Once);
            _lootTableRepositoryMock.Verify(library => library.Get(ITEM_ID), Times.Once);
            _lootTableRepositoryMock.VerifyNoOtherCalls();
            
            _inventoryUpdateDispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdate>()), Times.Once);
        }

        [Test]
        public void Negative_DispatchInventoryUpdates_ItemNotFound_Throws()
        {
            _lootTableRepositoryMock.Setup(library => library.Contains(ITEM_ID)).Returns(false);
            
            Assert.Throws<NotFoundException<ItemID>>(() => _lootService.DispatchInventoryUpdates(ITEM_ID));
            
            _inventoryUpdateDispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdate>()), Times.Never);
        }
    }
}