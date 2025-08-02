using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Services;
using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace ContentEngine.Tests.Service
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
            _skillNodeEntity = new SkillNodeEntity(new SkillComponent() { SkillID = SkillID.FARMING},[new ResourceComponent() { ResourceID = ResourceID.COPPER}]);
            _repositoryMock = new Mock<IAssetRepository<SkillID, SkillNodeEntity>>();
            _skillNodeAccessValidator = new SkillNodeAccessValidator(_repositoryMock.Object, new FoundAssertion(new ThrowHandler()));
            
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _repositoryMock.Setup(library => library.Get(SkillID.FARMING)).Returns(_skillNodeEntity);
        }

        [Test]
        public void Positive_AssertSkillAllows_SkillAllowsResource_NoThrow()
        {
            _repositoryMock.Setup(library => library.Contains(SkillID.FARMING)).Returns(true);
            
            Assert.DoesNotThrow(() => _skillNodeAccessValidator.AssertSkillAllows(SkillID.FARMING, ResourceID.COPPER));
            
            _repositoryMock.Verify(library => library.Get(SkillID.FARMING), Times.Once);
            _repositoryMock.Verify(library => library.Contains(SkillID.FARMING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_AssertSkillAllows_SkillNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(SkillID.FARMING)).Returns(false);
            
            Assert.Throws<NotFoundException<SkillID>>(() => _skillNodeAccessValidator.AssertSkillAllows(SkillID.FARMING,  ResourceID.COPPER));
            
            _repositoryMock.Verify(library => library.Get(SkillID.FARMING), Times.Never);
            _repositoryMock.Verify(library => library.Contains(SkillID.FARMING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_AssertSkillAllows_ResourceNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(SkillID.FARMING)).Returns(true);
            
            Assert.Throws<NotFoundException<ResourceID>>(() => _skillNodeAccessValidator.AssertSkillAllows(SkillID.FARMING,  ResourceID.IRON));
            
            _repositoryMock.Verify(library => library.Get(SkillID.FARMING), Times.Once);
            _repositoryMock.Verify(library => library.Contains(SkillID.FARMING), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
    }
}