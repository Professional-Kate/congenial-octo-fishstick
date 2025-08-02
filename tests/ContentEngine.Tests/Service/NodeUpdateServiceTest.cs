using ContentEngine.Runtime.Services;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Level;
using IdelPog.Common.Repository;
using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace ContentEngine.Tests.Service
{
    [TestFixture]
    public class NodeUpdateServiceTest
    {
        private INodeUpdateService _nodeUpdateService;
        private Mock<IStateRepository<ResourceID, HarvestNode>> _nodeRepositoryMock;
        private Mock<ILevelService> _levelServiceMock;
        private Mock<IHarvestNodeUpdateResponseFactory> _updateResponseFactoryMock;

        private HarvestNode _harvestNode;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _nodeRepositoryMock = new Mock<IStateRepository<ResourceID, HarvestNode>>();
            _levelServiceMock = new Mock<ILevelService>();
            _updateResponseFactoryMock = new Mock<IHarvestNodeUpdateResponseFactory>();

            _harvestNode = new HarvestNode
            {
                ResourceID = ResourceID.STONE,
                Information = new Information { Description = "", Name = "" },
                Levelable = new Levelable(0, 0, 0, 0)
            };
            
            _nodeUpdateService = new NodeUpdateService(_nodeRepositoryMock.Object, _levelServiceMock.Object, _updateResponseFactoryMock.Object, new FoundAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _nodeRepositoryMock.Reset();
            _levelServiceMock.Reset();
            _updateResponseFactoryMock.Reset();
        }

        private void AssertMockCalls(Times times, bool canLevel = false)
        {
            _nodeRepositoryMock.Verify(library => library.Get(ResourceID.STONE), times);
            _nodeRepositoryMock.Verify(library => library.Update(ResourceID.STONE, _harvestNode), times);
            _nodeRepositoryMock.VerifyNoOtherCalls();

            _levelServiceMock.Verify(library => library.CanLevel(_harvestNode.Levelable), times);
            _levelServiceMock.VerifyNoOtherCalls();
            
            _updateResponseFactoryMock.Verify(library => library.Create(_harvestNode, canLevel), times);
            _updateResponseFactoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_UpdateHarvestNode_AddsExperience_NoLevelUp()
        {
            _nodeRepositoryMock.Setup(library => library.Contains(ResourceID.STONE)).Returns(true);
            _nodeRepositoryMock.Setup(library => library.Get(ResourceID.STONE)).Returns(_harvestNode);
            
            Assert.DoesNotThrow(() => _nodeUpdateService.UpdateHarvestNode(ResourceID.STONE));
            
            _nodeRepositoryMock.Verify(library => library.Contains(ResourceID.STONE), Times.Once);
            _levelServiceMock.Verify(library => library.LevelUp(_harvestNode.Levelable), Times.Never);
            AssertMockCalls(Times.Once());
        }

        [Test]
        public void Positive_UpdateHarvestNode_NodeCanLevel_LevelsUp()
        {
            _nodeRepositoryMock.Setup(library => library.Contains(ResourceID.STONE)).Returns(true);
            _nodeRepositoryMock.Setup(library => library.Get(ResourceID.STONE)).Returns(_harvestNode);
            _levelServiceMock.Setup(library => library.CanLevel(_harvestNode.Levelable)).Returns(true);
            
            Assert.DoesNotThrow(() => _nodeUpdateService.UpdateHarvestNode(ResourceID.STONE));
            
            _nodeRepositoryMock.Verify(library => library.Contains(ResourceID.STONE), Times.Once);
            _levelServiceMock.Verify(library => library.LevelUp(_harvestNode.Levelable), Times.Once);
            AssertMockCalls(Times.Once(), canLevel: true);
        }

        [Test]
        public void Negative_UpdateHarvestNode_NodeNotFound_Throws()
        {
            _nodeRepositoryMock.Setup(library => library.Contains(ResourceID.STONE)).Returns(false);
            
            Assert.Throws<NotFoundException<ResourceID>>(() => _nodeUpdateService.UpdateHarvestNode(ResourceID.STONE));

            _nodeRepositoryMock.Verify(library => library.Contains(ResourceID.STONE), Times.Once);
            AssertMockCalls(Times.Never());
        }
    }
}