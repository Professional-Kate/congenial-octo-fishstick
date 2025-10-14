using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.Progression.Runtime;
using IdelPog.Progression.Runtime.Component;
using IdelPog.Progression.Runtime.System;
using IdelPog.Progression.Runtime.System.Interface;
using Moq;

namespace IdelPog.Progression.Tests.System
{
    [TestFixture]
    public class EntityUnlockCheckerTest
    {
        private IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse> _unlockChecker;
        private Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>> _repositoryMock;

        private UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> _unlockRequirementsEntity;
        private HarvestNodeUnlockResponse _foragingResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            _repositoryMock = new Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>>();
            
            _unlockChecker = new EntityUnlockChecker<SkillID, HarvestNodeUnlockResponse>(_repositoryMock.Object);

            _foragingResponse = new HarvestNodeUnlockResponse { SkillID = SkillID.FORAGING, ResourceID = ResourceID.RIVER };
            _unlockRequirementsEntity = new UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>(repositoryAsserter, [new LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> { ID = SkillID.FORAGING, Level = 1, OnUnlockCommand = _foragingResponse }
            ]);
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
        }

        [Test]
        public void Positive_IsUnlocked_EntityNotFound_ReturnsTrue()
        {
            _repositoryMock.Setup(library => library.Contains(_foragingResponse.SkillID)).Returns(false);
            
            bool unlocked = _unlockChecker.IsUnlocked(_foragingResponse.SkillID, component => component.OnUnlockCommand.ResourceID == ResourceID.BEEHIVE);
            
            Assert.That(unlocked, Is.True);
            
            _repositoryMock.Verify(library => library.Contains(_foragingResponse.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_IsUnlocked_EntityFound_ComponentNotFound_ReturnsTrue()
        {
            _repositoryMock.Setup(library => library.Contains(_foragingResponse.SkillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_foragingResponse.SkillID)).Returns(_unlockRequirementsEntity);
            
            bool unlocked = _unlockChecker.IsUnlocked(_foragingResponse.SkillID, component => component.OnUnlockCommand.ResourceID == ResourceID.BEEHIVE);
            
            Assert.That(unlocked, Is.True);
            
            _repositoryMock.Verify(library => library.Contains(_foragingResponse.SkillID), Times.Once);
            _repositoryMock.Verify(library => library.Get(_foragingResponse.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_IsUnlocked_EntityFound_ComponentFound_ReturnsFalse()
        {
            _repositoryMock.Setup(library => library.Contains(_foragingResponse.SkillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_foragingResponse.SkillID)).Returns(_unlockRequirementsEntity);
            
            bool unlocked = _unlockChecker.IsUnlocked(_foragingResponse.SkillID, component => component.OnUnlockCommand.ResourceID == _foragingResponse.ResourceID);
            
            Assert.That(unlocked, Is.False);
            
            _repositoryMock.Verify(library => library.Contains(_foragingResponse.SkillID), Times.Once);
            _repositoryMock.Verify(library => library.Get(_foragingResponse.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
    }
}