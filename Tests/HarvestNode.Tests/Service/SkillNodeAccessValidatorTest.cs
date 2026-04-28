using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Service
{
    [TestFixture]
    public sealed class SkillNodeAccessValidatorTest
    {
        private ISkillNodeAccessValidator _skillNodeAccessValidator;
        private Mock<IAssetRepository<SkillID, SkillNodeEntity>> _repositoryMock;
        
        private SkillNodeEntity _skillNodeEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            _skillNodeEntity = new SkillNodeEntity(repositoryAsserter,[new HarvestTargetComponent { HarvestTarget = ResourceID.COPPER_CLUSTER}]) { SkillID = SkillID.FORAGING };
            _repositoryMock = new Mock<IAssetRepository<SkillID, SkillNodeEntity>>();
            
            _skillNodeAccessValidator = new SkillNodeAccessValidator(_repositoryMock.Object, new FoundAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _repositoryMock.Setup(library => library.Get(_skillNodeEntity.SkillID)).Returns(_skillNodeEntity);
        }

        [Test]
        public void Positive_AssertSkillAllows_SkillAllowsResource_NoThrow()
        {
            _repositoryMock.Setup(library => library.Contains(_skillNodeEntity.SkillID)).Returns(true);
            
            Assert.DoesNotThrow(() => _skillNodeAccessValidator.AssertSkillAllows(_skillNodeEntity.SkillID, ResourceID.COPPER_CLUSTER));
            
            _repositoryMock.Verify(library => library.Get(_skillNodeEntity.SkillID), Times.Once);
            _repositoryMock.Verify(library => library.Contains(_skillNodeEntity.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_AssertSkillAllows_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_skillNodeEntity.SkillID)).Returns(false);
            
            Assert.Throws<NotFoundException<SkillID>>(() => _skillNodeAccessValidator.AssertSkillAllows(_skillNodeEntity.SkillID,  ResourceID.COPPER_CLUSTER));
            
            _repositoryMock.Verify(library => library.Get(_skillNodeEntity.SkillID), Times.Never);
            _repositoryMock.Verify(library => library.Contains(_skillNodeEntity.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_AssertSkillAllows_ResourceNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_skillNodeEntity.SkillID)).Returns(true);
            
            Assert.Throws<NotFoundException<ResourceID>>(() => _skillNodeAccessValidator.AssertSkillAllows(_skillNodeEntity.SkillID,  ResourceID.IRON_CLUSTER));
            
            _repositoryMock.Verify(library => library.Get(_skillNodeEntity.SkillID), Times.Once);
            _repositoryMock.Verify(library => library.Contains(_skillNodeEntity.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
    }
}