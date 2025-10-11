using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Random;
using IdelPog.Loot.Table;
using Moq;

namespace IdelPog.HarvestNode.Tests.Service
{
    [TestFixture]
    public sealed class LootTableServiceTest
    {
        private ILootTableService<ResourceID> _lootTableService;
        private Mock<IAssetRepository<ResourceID, ILootTable>> _repositoryMock;
        private Mock<IWeightedLootTableFactory> _factoryMock;
        
        private LootTableEntry _honeyEntry;
        private WeightedEntry _honeyWeightedEntry;
        private const ResourceID RESOURCE_ID = ResourceID.RIVER;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<ResourceID, ILootTable>>();
            _factoryMock = new Mock<IWeightedLootTableFactory>();
            ThrowHandler throwHandler = new();
            
            _lootTableService = new LootTableService<ResourceID>(_repositoryMock.Object, _factoryMock.Object, new UniqueAssertion(throwHandler));

            _honeyEntry = new LootTableEntry { ItemID = ItemID.HONEY, Weight = 1 };
            _honeyWeightedEntry = new WeightedEntry { ItemID = ItemID.HONEY, Weight = 1 };
        }
        
        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _factoryMock.Reset();
        }

        private void VerifyRepositoryContainsCalled(ResourceID resourceID)
        {
            _repositoryMock.Verify(library => library.Contains(resourceID), Times.Once);
        }

        private void VerifyRepositoryAdd(ResourceID resourceID)
        {
            _repositoryMock.Verify(library => library.Add(resourceID, It.IsAny<ILootTable>()), Times.Once);
        }

        private void VerifyRepositoryNoMoreCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyFactoryCalled(params WeightedEntry[] weightedEntries)
        {
            _factoryMock.Verify(library => library.Create(weightedEntries, It.IsAny<ILootRoll>()), Times.Once);
        }

        private void VerifyFactoryNoMoreCalls()
        {
            _factoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_CreateLootTable_OneEntry_CreatesAndAddsTable_SkipsFactory()
        {
            Assert.DoesNotThrow(() => _lootTableService.CreateLootTable([_honeyEntry], RESOURCE_ID));
            
            VerifyRepositoryContainsCalled(RESOURCE_ID);
            VerifyRepositoryAdd(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryNoMoreCalls();
        }

        [Test]
        public void Positive_CreateLootTable_CreatesAndAddsTable()
        {
            LootTableEntry ironEntry = _honeyEntry with { ItemID = ItemID.IRON };
            WeightedEntry ironWeightedEntry = _honeyWeightedEntry with { ItemID = ItemID.IRON };
            
            Assert.DoesNotThrow(() => _lootTableService.CreateLootTable([_honeyEntry, ironEntry], RESOURCE_ID));
            
            VerifyRepositoryContainsCalled(RESOURCE_ID);
            VerifyRepositoryAdd(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryCalled(_honeyWeightedEntry, ironWeightedEntry);
            VerifyFactoryNoMoreCalls();
        }

        [Test]
        public void Negative_CreateLootTable_ResourceID_NonUnique_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(RESOURCE_ID)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _lootTableService.CreateLootTable([_honeyEntry], RESOURCE_ID));
            
            VerifyRepositoryContainsCalled(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryNoMoreCalls();
        }
    }
}