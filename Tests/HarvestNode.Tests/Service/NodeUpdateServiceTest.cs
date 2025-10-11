using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Service
{
    [TestFixture]
    public class NodeUpdateServiceTest
    {
        private INodeUpdateService _nodeUpdateService;
        private Mock<IStateRepository<ResourceID, Contracts.HarvestNode>> _nodeRepositoryMock;
        private Mock<ILevelService> _levelServiceMock;
        private Mock<IExperienceService> _experienceServiceMock;
        private Mock<INodeUpdateResponseFactory> _updateResponseFactoryMock;

        private Contracts.HarvestNode _harvestNode;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _nodeRepositoryMock = new Mock<IStateRepository<ResourceID, Contracts.HarvestNode>>();
            _levelServiceMock = new Mock<ILevelService>();
            _experienceServiceMock = new Mock<IExperienceService>();
            _updateResponseFactoryMock = new Mock<INodeUpdateResponseFactory>();

            _harvestNode = new Contracts.HarvestNode
            {
                Information = new Information { Description = "", Name = "" },
                Levelable = new Levelable(0, 0, 0, 0),
                ResourceID = ResourceID.STONE, 
                LocationID = LocationID.CAVE
            };
            
            _nodeUpdateService = new NodeUpdateService(_nodeRepositoryMock.Object, _levelServiceMock.Object, _experienceServiceMock.Object, _updateResponseFactoryMock.Object, new FoundAssertion(new ThrowHandler()));
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