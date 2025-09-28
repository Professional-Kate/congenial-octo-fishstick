using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Contracts;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.ECS;
using IdelPog.Progression.Runtime.ECS.Component;
using IdelPog.Progression.Runtime.ECS.System;
using IdelPog.Progression.Runtime.ECS.System.Interface;
using Moq;

namespace IdelPog.Progression.Tests.System
{
    [TestFixture]
    public sealed class NodeUnlockerServiceTest
    {
        private INodeUnlockerService _nodeUnlockerService;
        private Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<HarvestNodeUnlockResponse>>> _repositoryMock;
        private HarvestNodeUnlock _harvestNodeUnlock;
        private HarvestNodeUnlockResponse _harvestNodeUnlockResponse;
        private UnlockRequirementsEntity<HarvestNodeUnlockResponse> _harvestNodeUnlockEntity;
        private NodeLevelRequirement<HarvestNodeUnlockResponse> _nodeLevelRequirement;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ThrowHandler throwHandler = new();
            _repositoryMock = new Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<HarvestNodeUnlockResponse>>>();
            
            _nodeUnlockerService = new NodeUnlockerService(_repositoryMock.Object, new FoundAssertion(throwHandler), new CanUnlockAssertion<HarvestNodeUnlockResponse>(throwHandler), new SkillMatchesAssertion(throwHandler), new QueueAssertion<HarvestNodeUnlockResponse>(throwHandler));

            _harvestNodeUnlock = new HarvestNodeUnlock { SkillID = SkillID.MINING, SkillLevel = 5 };
            _harvestNodeUnlockResponse = new HarvestNodeUnlockResponse { ItemID = ItemID.BIRCH, SkillID = SkillID.MINING, SkillLevel = 5 };
            _nodeLevelRequirement = new NodeLevelRequirement<HarvestNodeUnlockResponse> { Level = 5, SkillID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse };
            _harvestNodeUnlockEntity = new UnlockRequirementsEntity<HarvestNodeUnlockResponse>([_nodeLevelRequirement]);
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _harvestNodeUnlockEntity = new UnlockRequirementsEntity<HarvestNodeUnlockResponse>([_nodeLevelRequirement]);
        }

        private void SetupRepository(SkillID skillID)
        {
            _repositoryMock.Setup(library => library.Contains(skillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(skillID)).Returns(_harvestNodeUnlockEntity);
        }
        
        private void VerifyRepository(SkillID skillID)
        {
            _repositoryMock.Verify(library => library.Contains(skillID), Times.Once);
            _repositoryMock.Verify(library => library.Get(skillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyResponse(HarvestNodeUnlock harvestNodeUnlock, HarvestNodeUnlockResponse harvestNodeUnlockResponse)
        {
            Assert.Multiple(() =>
            {
                Assert.That(harvestNodeUnlock.SkillID, Is.EqualTo(harvestNodeUnlockResponse.SkillID));
                Assert.That(harvestNodeUnlock.SkillLevel, Is.EqualTo(harvestNodeUnlockResponse.SkillLevel));
                Assert.That(harvestNodeUnlockResponse.ItemID, Is.EqualTo(_harvestNodeUnlockResponse.ItemID));
            });
        }

        [TestCase(5, true)]
        [TestCase(1, false)]
        [TestCase(0, false)]
        public void Positive_CanUnlock_ReturnsExpectedBool(byte level, bool expected)
        {
            SetupRepository(_harvestNodeUnlock.SkillID);

            bool canUnlock = _nodeUnlockerService.CanUnlock(_harvestNodeUnlock with { SkillLevel = level });
            
            Assert.That(canUnlock, Is.EqualTo(expected));
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_CanUnlock_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(false);

            Assert.Throws<NotFoundException<SkillID>>(() => _nodeUnlockerService.CanUnlock(_harvestNodeUnlock));
            
            _repositoryMock.Verify(library => library.Contains(_harvestNodeUnlock.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_CanUnlock_SkillMismatch_Throws()
        {
            SetupRepository(SkillID.FORAGING);
            
            Assert.Throws<SkillMismatchException>(() => _nodeUnlockerService.CanUnlock(_harvestNodeUnlock with { SkillID = SkillID.FORAGING }));
            
            VerifyRepository(SkillID.FORAGING);
        }

        [Test]
        public void Positive_Unlock_ReturnsResponse()
        {
            SetupRepository(_harvestNodeUnlock.SkillID);
            
            HarvestNodeUnlockResponse response = _nodeUnlockerService.Unlock(_harvestNodeUnlock);
            
            VerifyResponse(_harvestNodeUnlock, response);
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_Unlock_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(false);

            Assert.Throws<NotFoundException<SkillID>>(() => _nodeUnlockerService.Unlock(_harvestNodeUnlock));
            
            _repositoryMock.Verify(library => library.Contains(_harvestNodeUnlock.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_Unlock_SkillMismatch_Throws()
        {
            SetupRepository(SkillID.FORAGING);
            
            Assert.Throws<SkillMismatchException>(() => _nodeUnlockerService.Unlock(_harvestNodeUnlock with { SkillID = SkillID.FORAGING }));
            
            VerifyRepository(SkillID.FORAGING);
        }

        [Test]
        public void Negative_Unlock_CannotUnlock_Throws()
        {
            SetupRepository(_harvestNodeUnlock.SkillID);

            Assert.Throws<CannotUnlockException<HarvestNodeUnlockResponse>>(() => _nodeUnlockerService.Unlock(_harvestNodeUnlock with { SkillLevel = 1 }));
            
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_Unlock_EmptyEntity_Throws()
        {
            _harvestNodeUnlockEntity.TryDequeue(out NodeLevelRequirement<HarvestNodeUnlockResponse> _);
            
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_harvestNodeUnlock.SkillID)).Returns(_harvestNodeUnlockEntity);

            Assert.Throws<InvalidOperationException>(() => _nodeUnlockerService.Unlock(_harvestNodeUnlock));
            
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }
    }
}