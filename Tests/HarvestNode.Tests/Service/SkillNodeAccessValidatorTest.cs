using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Service
{
    [TestFixture]
    public class SkillNodeAccessValidatorTest
    {
        private ISkillNodeAccessValidator _skillNodeAccessValidator;
        private Mock<IAssetRepository<SkillID, SkillNodeEntity>> _repositoryMock;
        
        private SkillNodeEntity _skillNodeEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillNodeEntity = new SkillNodeEntity(new SkillComponent() { SkillID = SkillID.FORAGING},[new HarvestTargetComponent() { HarvestTarget = ResourceID.COPPER_CLUSTER}]);
            _repositoryMock = new Mock<IAssetRepository<SkillID, SkillNodeEntity>>();
            _skillNodeAccessValidator = new SkillNodeAccessValidator(_repositoryMock.Object, new FoundAssertion(new ThrowHandler()));
            
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _repositoryMock.Setup(library => library.Get(SkillID.FORAGING)).Returns(_skillNodeEntity);
        }

        [Test]
        public void Positive_AssertSkillAllows_SkillAllowsResource_NoThrow()
        {
            _repositoryMock.Setup(library => library.Contains(SkillID.FORAGING)).Returns(true);
            
            Assert.DoesNotThrow(() => _skillNodeAccessValidator.AssertSkillAllows(SkillID.FORAGING, ResourceID.COPPER_CLUSTER));
            
            _repositoryMock.Verify(library => library.Get(SkillID.FORAGING), Times.Once);
            _repositoryMock.Verify(library => library.Contains(SkillID.FORAGING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_AssertSkillAllows_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(SkillID.FORAGING)).Returns(false);
            
            Assert.Throws<NotFoundException<SkillID>>(() => _skillNodeAccessValidator.AssertSkillAllows(SkillID.FORAGING,  ResourceID.COPPER_CLUSTER));
            
            _repositoryMock.Verify(library => library.Get(SkillID.FORAGING), Times.Never);
            _repositoryMock.Verify(library => library.Contains(SkillID.FORAGING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_AssertSkillAllows_ResourceNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(SkillID.FORAGING)).Returns(true);
            
            Assert.Throws<NotFoundException<ResourceID>>(() => _skillNodeAccessValidator.AssertSkillAllows(SkillID.FORAGING,  ResourceID.IRON_CLUSTER));
            
            _repositoryMock.Verify(library => library.Get(SkillID.FORAGING), Times.Once);
            _repositoryMock.Verify(library => library.Contains(SkillID.FORAGING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
    }
}