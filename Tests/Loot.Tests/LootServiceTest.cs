using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Service;
using IdelPog.Loot.Service.Interface;
using IdelPog.Loot.Table;
using Moq;

namespace Loot.Tests
{
    [TestFixture]
    public class LootServiceTest
    {
        private ILootService<ItemID> _lootService;
        private Mock<IAssetRepository<ItemID, ILootTable>> _lootTableRepositoryMock;
        private Mock<ILootTable> _weightedLootTableMock;
        private Mock<IGrantPolicy> _grantPolicyMock;

        private const ItemID ITEM_ID = ItemID.STONE;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootTableRepositoryMock = new Mock<IAssetRepository<ItemID, ILootTable>>();
            _weightedLootTableMock = new Mock<ILootTable>();
            _grantPolicyMock = new Mock<IGrantPolicy>();
            
            _lootService = new LootService<ItemID>(_lootTableRepositoryMock.Object, _grantPolicyMock.Object, new FoundAssertion(new ThrowHandler()));
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
            _grantPolicyMock.Setup(library => library.ShouldGrant()).Returns(true);
            
            Assert.DoesNotThrow(() => _lootService.GenerateItemID(ITEM_ID));
            
            _weightedLootTableMock.Verify(library => library.Roll(), Times.Once);
            
            _lootTableRepositoryMock.Verify(library => library.Contains(ITEM_ID), Times.Once);
            _lootTableRepositoryMock.Verify(library => library.Get(ITEM_ID), Times.Once);
            _lootTableRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_DispatchInventoryUpdates_ItemNotFound_Throws()
        {
            _lootTableRepositoryMock.Setup(library => library.Contains(ITEM_ID)).Returns(false);
            _grantPolicyMock.Setup(library => library.ShouldGrant()).Returns(true);
            
            Assert.Throws<NotFoundException<ItemID>>(() => _lootService.GenerateItemID(ITEM_ID));
            
            _lootTableRepositoryMock.Verify(library => library.Contains(ITEM_ID), Times.Once);
            _lootTableRepositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Positive_GrantPolicyReturns_ReturnsThatValue(bool granted)
        {
            _grantPolicyMock.Setup(library => library.ShouldGrant()).Returns(granted);
            bool shouldGrant = _lootService.ShouldGrant();
            
            Assert.That(shouldGrant, Is.EqualTo(granted));
            _lootTableRepositoryMock.VerifyNoOtherCalls();
        }
    }
}