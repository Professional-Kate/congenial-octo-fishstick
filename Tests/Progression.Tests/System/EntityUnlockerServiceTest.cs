using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime;
using IdelPog.Progression.Runtime.Component;
using IdelPog.Progression.Runtime.System;
using IdelPog.Progression.Runtime.System.Interface;
using Moq;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace IdelPog.Progression.Tests.System
{
    [TestFixture]
    public sealed class EntityUnlockerServiceTest
    {
        private IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse> _entityUnlockerService;
        private Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>> _repositoryMock;
        private HarvestNodeUnlock _harvestNodeUnlock;
        private HarvestNodeUnlockResponse _harvestNodeUnlockResponse;
        private UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> _harvestNodeUnlockEntity;
        private LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> _levelRequirementComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>>();
            
            _entityUnlockerService = new EntityUnlockerService<SkillID, HarvestNodeUnlockResponse>(_repositoryMock.Object, new FoundAssertion(), new CanUnlockAssertion<SkillID, HarvestNodeUnlockResponse>(), new IDMatchesAssertion<SkillID>(), new QueueAssertion<SkillID, HarvestNodeUnlockResponse>());

            _harvestNodeUnlock = new HarvestNodeUnlock { SkillID = SkillID.MINING, SkillLevel = 5 };
            _harvestNodeUnlockResponse = new HarvestNodeUnlockResponse { ResourceID = ResourceID.COPPER_CLUSTER, SkillID = SkillID.MINING };
            _levelRequirementComponent = new LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> { Level = 5, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse };
            _harvestNodeUnlockEntity = new UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>([_levelRequirementComponent]);
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _harvestNodeUnlockEntity = new UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>([_levelRequirementComponent]);
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
                Assert.That(harvestNodeUnlockResponse.ResourceID, Is.EqualTo(_harvestNodeUnlockResponse.ResourceID));
            });
        }

        [TestCase(5, true)]
        [TestCase(1, false)]
        [TestCase(0, false)]
        public void Positive_CanUnlock_ReturnsExpectedBool(byte level, bool expected)
        {
            SetupRepository(_harvestNodeUnlock.SkillID);

            bool canUnlock = _entityUnlockerService.CanUnlock(_harvestNodeUnlock.SkillID, level);
            
            Assert.That(canUnlock, Is.EqualTo(expected));
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_CanUnlock_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(false);

            Assert.Throws<NotFoundException<SkillID>>(() => _entityUnlockerService.CanUnlock(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel));
            
            _repositoryMock.Verify(library => library.Contains(_harvestNodeUnlock.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_CanUnlock_SkillMismatch_Throws()
        {
            SetupRepository(SkillID.FORAGING);
            
            Assert.Throws<IDMismatchException<SkillID>>(() => _entityUnlockerService.CanUnlock(SkillID.FORAGING, _harvestNodeUnlock.SkillLevel));
            
            VerifyRepository(SkillID.FORAGING);
        }

        [Test]
        public void Positive_Unlock_ReturnsResponse()
        {
            SetupRepository(_harvestNodeUnlock.SkillID);
            
            HarvestNodeUnlockResponse response = _entityUnlockerService.Unlock(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel);
            
            VerifyResponse(_harvestNodeUnlock, response);
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_Unlock_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(false);

            Assert.Throws<NotFoundException<SkillID>>(() => _entityUnlockerService.Unlock(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel));
            
            _repositoryMock.Verify(library => library.Contains(_harvestNodeUnlock.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_Unlock_SkillMismatch_Throws()
        {
            SetupRepository(SkillID.FORAGING);
            
            Assert.Throws<IDMismatchException<SkillID>>(() => _entityUnlockerService.Unlock(SkillID.FORAGING, _harvestNodeUnlock.SkillLevel));
            
            VerifyRepository(SkillID.FORAGING);
        }

        [Test]
        public void Negative_Unlock_CannotUnlock_Throws()
        {
            SetupRepository(_harvestNodeUnlock.SkillID);

            Assert.Throws<CannotUnlockException<SkillID, HarvestNodeUnlockResponse>>(() => _entityUnlockerService.Unlock(_harvestNodeUnlock.SkillID, 1));
            
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_Unlock_EmptyEntity_Throws()
        {
            _harvestNodeUnlockEntity.TryDequeue(out LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> _);
            
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_harvestNodeUnlock.SkillID)).Returns(_harvestNodeUnlockEntity);

            Assert.Throws<InvalidOperationException>(() => _entityUnlockerService.Unlock(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel));
            
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Positive_UnlockAvailable_UnlocksTillLevel()
        {
            LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>[] unlockComponents =
            [
                new() { Level = 1, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse },
                new() { Level = 2, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse },
                new() { Level = 3, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse },
                new() { Level = 4, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse },
                new() { Level = 5, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse },
                new() { Level = 6, ID = SkillID.MINING, OnUnlockCommand = _harvestNodeUnlockResponse }
            ];
            
            UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> entity = new(unlockComponents);
            
            _repositoryMock.Setup(library => library.Contains(SkillID.MINING)).Returns(true);
            _repositoryMock.Setup(library => library.Get(SkillID.MINING)).Returns(entity);
            
            HarvestNodeUnlockResponse[] responses = _entityUnlockerService.UnlockAllAvailable(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel).ToArray();
            
            Assert.That(responses.Count, Is.EqualTo(unlockComponents.Length - 1));
            
            _repositoryMock.Verify(library => library.Contains(SkillID.MINING), Times.Once);
            _repositoryMock.Verify(library => library.Get(SkillID.MINING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
            
            foreach (HarvestNodeUnlockResponse harvestNodeUnlockResponse in responses)
            {
                VerifyResponse(_harvestNodeUnlock, harvestNodeUnlockResponse);
            }
        }
        
        [Test]
        public void Positive_UnlockAllAvailable_CannotUnlock_DoesNotReturn()
        {
            SetupRepository(_harvestNodeUnlock.SkillID);

            HarvestNodeUnlockResponse[] responses = _entityUnlockerService.UnlockAllAvailable(_harvestNodeUnlock.SkillID, 1).ToArray();
            
            Assert.That(responses.Count, Is.EqualTo(0));
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }

        [Test]
        public void Negative_UnlockAllAvailable_SkillNotFound_Throws()
        { 
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(false);

            Assert.Throws<NotFoundException<SkillID>>(() => _entityUnlockerService.UnlockAllAvailable(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel).ToArray());
            
            _repositoryMock.Verify(library => library.Contains(_harvestNodeUnlock.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_UnlockAllAvailable_MismatchSkillID_Throws()
        {
            SetupRepository(SkillID.FORAGING);
            
            Assert.Throws<IDMismatchException<SkillID>>(() => _entityUnlockerService.UnlockAllAvailable(SkillID.FORAGING, _harvestNodeUnlock.SkillLevel).ToArray());
            
            VerifyRepository(SkillID.FORAGING);
        }

        [Test]
        public void Negative_UnlockAllAvailable_EmptyEntity_Throws()
        {
            _harvestNodeUnlockEntity.TryDequeue(out LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> _);
            
            _repositoryMock.Setup(library => library.Contains(_harvestNodeUnlock.SkillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_harvestNodeUnlock.SkillID)).Returns(_harvestNodeUnlockEntity);

            Assert.Throws<InvalidOperationException>(() => _entityUnlockerService.UnlockAllAvailable(_harvestNodeUnlock.SkillID, _harvestNodeUnlock.SkillLevel).ToArray());
            
            VerifyRepository(_harvestNodeUnlock.SkillID);
        }
    }
}