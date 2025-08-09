using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Factory.Interfaces;
using ContentEngine.Runtime.Mediator;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace ContentEngine.Tests.Mediator
{
    [TestFixture]
    public class NodeCreationMediatorTest
    {
        private IBatchMediator<NodeCreation> _nodeCreationMediator;
        private Mock<IAssetRepository<SkillID, SkillNodeEntity>> _skillNodeEntityRepositoryMock;
        private Mock<IStateRepository<ResourceID, HarvestNode>> _harvestNodeRepositoryMock;
        private Mock<ISkillNodeEntityFactory> _skillNodeEntityFactoryMock;
        private Mock<IHarvestNodeFactory> _harvestNodeFactoryMock;
        private Mock<INodeCreationResponseFactory> _nodeCreationResponseFactoryMock;
        private Mock<IDispatchOne<NodeCreationResponse>> _dispatchOneMock;

        private NodeCreation _miningCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillNodeEntityRepositoryMock = new Mock<IAssetRepository<SkillID, SkillNodeEntity>>();
            _harvestNodeRepositoryMock = new Mock<IStateRepository<ResourceID, HarvestNode>>();
            _skillNodeEntityFactoryMock = new Mock<ISkillNodeEntityFactory>();
            _harvestNodeFactoryMock = new Mock<IHarvestNodeFactory>();
            _nodeCreationResponseFactoryMock = new Mock<INodeCreationResponseFactory>();
            _dispatchOneMock = new Mock<IDispatchOne<NodeCreationResponse>>();

            _miningCreation = new NodeCreation
            {
                ResourceIDs = [ResourceID.STONE, ResourceID.COPPER, ResourceID.GOLD, ResourceID.IRON],
                LinkedSkill = SkillID.MINING
            };

            _nodeCreationMediator = new NodeCreationMediator(_harvestNodeRepositoryMock.Object, _skillNodeEntityRepositoryMock.Object, _skillNodeEntityFactoryMock.Object, _harvestNodeFactoryMock.Object, _nodeCreationResponseFactoryMock.Object, _dispatchOneMock.Object, new UniqueAssertion(new ThrowHandler()), new CollectionAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _skillNodeEntityRepositoryMock.Reset();
            _harvestNodeRepositoryMock.Reset();
            _skillNodeEntityFactoryMock.Reset();
            _harvestNodeFactoryMock.Reset();
            _nodeCreationResponseFactoryMock.Reset();
            _dispatchOneMock.Reset();
        }

        [Test]
        public void Positive_HandleMessages_SkillNotFound_CreatesSkillAndNodes()
        {
            Assert.DoesNotThrow(() => _nodeCreationMediator.HandleMessages([_miningCreation]));
            
            _skillNodeEntityRepositoryMock.Verify(library => library.Add(_miningCreation.LinkedSkill, It.IsAny<SkillNodeEntity>()), Times.Once);
            _harvestNodeRepositoryMock.Verify(library => library.Add(It.IsIn(_miningCreation.ResourceIDs), It.IsAny<HarvestNode>()), Times.Exactly(_miningCreation.ResourceIDs.Length));
            _skillNodeEntityFactoryMock.Verify(library => library.Create(_miningCreation.LinkedSkill, _miningCreation.ResourceIDs), Times.Once);
            _harvestNodeFactoryMock.Verify(library => library.Create(It.IsIn(_miningCreation.ResourceIDs)), Times.Exactly(_miningCreation.ResourceIDs.Length));
            _dispatchOneMock.Verify(library => library.Dispatch(It.IsAny<NodeCreationResponse>()), Times.Once);
        }

        [Test]
        public void Negative_HandleMessages_SkillAlreadyExists_Throws()
        {
            _skillNodeEntityRepositoryMock.Setup(library => library.Contains(It.IsAny<SkillID>())).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _nodeCreationMediator.HandleMessages([_miningCreation]));
            
            _dispatchOneMock.Verify(library => library.Dispatch(It.IsAny<NodeCreationResponse>()), Times.Never);
            _skillNodeEntityRepositoryMock.Verify(library => library.Add(_miningCreation.LinkedSkill, It.IsAny<SkillNodeEntity>()), Times.Never);
            _harvestNodeRepositoryMock.Verify(library => library.Add(It.IsIn(_miningCreation.ResourceIDs), It.IsAny<HarvestNode>()), Times.Never);
        }

        [Test]
        public void Negative_HandleMessages_ResourceIDAlreadyExists_Throws()
        {
            _harvestNodeRepositoryMock.Setup(library => library.Contains(It.IsAny<ResourceID>())).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _nodeCreationMediator.HandleMessages([_miningCreation]));
            
            _dispatchOneMock.Verify(library => library.Dispatch(It.IsAny<NodeCreationResponse>()), Times.Never);
            _skillNodeEntityRepositoryMock.Verify(library => library.Add(_miningCreation.LinkedSkill, It.IsAny<SkillNodeEntity>()), Times.Never);
            _harvestNodeRepositoryMock.Verify(library => library.Add(It.IsIn(_miningCreation.ResourceIDs), It.IsAny<HarvestNode>()), Times.Never);
            
            
        }
    }
}