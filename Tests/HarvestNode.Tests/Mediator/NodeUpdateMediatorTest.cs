using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Assertion;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Exceptions;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Progression.Runtime.Component;
using IdelPog.Progression.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public sealed class NodeUpdateMediatorTest
    {
        private IBatchMediator<HarvestNodeUpdate> _updateMediator;
        private Mock<ISkillNodeAccessValidator> _skillNodeAccessValidatorMock;
        private Mock<INodeUpdateService> _nodeUpdateServiceMock;
        private Mock<IDispatchMany<HarvestNodeUpdateResponse>> _responseDispatcherMock;
        private Mock<IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse>> _checkerMock;
        private Mock<IStateRepository<ResourceID, Contracts.HarvestNode>> _harvestNodeRepository;
        private Mock<IDispatchMany<InventoryUpdate>> _inventoryUpdateDispatcherMock;
        private Mock<IHarvestNodeLootService> _harvestNodeLootServiceMock;
        
        private Contracts.HarvestNode _harvestNode;
        private HarvestNodeUpdate _nodeUpdate;
        private HarvestNodeUpdateResponse _expectedResponse;
        private InventoryUpdate _stoneUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stoneUpdate = new InventoryUpdate
            {
                ActionType = ActionType.ADD,
                Amount = 1,
                ItemID = ItemID.STONE
            };
            
            _harvestNode = new Contracts.HarvestNode
            {
                Information = new Information { Description = "", Name = "" },
                Levelable = new Levelable(0, 0, 0, 0),
                ResourceID = ResourceID.STONE, 
                LocationID = LocationID.CAVE
            };
            
            _nodeUpdate = new HarvestNodeUpdate
            {
                SkillID = SkillID.MINING,
                ResourceID = ResourceID.STONE
            };

            _expectedResponse = new HarvestNodeUpdateResponse
            {
                HasLeveled = false,
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                ResourceID = _harvestNode.ResourceID,
                LocationID = _harvestNode.LocationID
            };

            ThrowHandler throwHandler = new();
            _skillNodeAccessValidatorMock = new Mock<ISkillNodeAccessValidator>();
            _nodeUpdateServiceMock = new Mock<INodeUpdateService>();
            _responseDispatcherMock = new Mock<IDispatchMany<HarvestNodeUpdateResponse>>();
            _checkerMock = new Mock<IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse>>();
            _harvestNodeRepository =  new Mock<IStateRepository<ResourceID, Contracts.HarvestNode>>();
            _inventoryUpdateDispatcherMock = new Mock<IDispatchMany<InventoryUpdate>>();
            _harvestNodeLootServiceMock =  new Mock<IHarvestNodeLootService>();
            
            _updateMediator = new NodeUpdateMediator(_harvestNodeRepository.Object, _skillNodeAccessValidatorMock.Object, _checkerMock.Object, _nodeUpdateServiceMock.Object, _harvestNodeLootServiceMock.Object, _responseDispatcherMock.Object, _inventoryUpdateDispatcherMock.Object, new NodeUnlockedAssertion(throwHandler), new CollectionAssertion(throwHandler), new FoundAssertion(throwHandler));
        }
        
        [SetUp]
        public void Setup()
        {
            _skillNodeAccessValidatorMock.Reset();
            _nodeUpdateServiceMock.Reset();
            _responseDispatcherMock.Reset();
            _checkerMock.Reset();
            _responseDispatcherMock.Reset();
            _harvestNodeLootServiceMock.Reset();
            _inventoryUpdateDispatcherMock.Reset();
        }

        private void SetupUnlockChecker(bool shouldReturn)
        {
            _checkerMock.Setup(library => library.IsUnlocked(_nodeUpdate.SkillID, It.IsAny< Predicate<LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>>>())).Returns(shouldReturn);
        }

        private void SetupRepositoryContains(bool contains, ResourceID resourceID)
        {
            _harvestNodeRepository.Setup(library => library.Contains(resourceID)).Returns(contains);
        }
        
        private void SetupRepositoryGet(Contracts.HarvestNode harvestNode)
        {
            _harvestNodeRepository.Setup(library => library.Get(harvestNode.ResourceID)).Returns(harvestNode);
        }

        private void SetupLootService(Contracts.HarvestNode harvestNode, params InventoryUpdate[] updates)
        {
            _harvestNodeLootServiceMock.Setup(library => library.GenerateInventoryUpdates(harvestNode)).Returns(updates);
        }

        private void AssertInventoryUpdateDispatcher(Times times)
        {
            _inventoryUpdateDispatcherMock.Verify(library => library.Dispatch(It.IsAny<IReadOnlyList<InventoryUpdate>>()), times);
        }

        private void AssertUpdateResponseDispatcher(Times times)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<IReadOnlyList<HarvestNodeUpdateResponse>>()), times);
        }
        
        [Test]
        public void Positive_HandleMessage_UpdateNode_DispatchesResponse()
        {
            SetupRepositoryContains(true, _harvestNode.ResourceID);
            SetupRepositoryGet(_harvestNode);
            SetupLootService(_harvestNode, _stoneUpdate);
            SetupUnlockChecker(true);
            
            Assert.DoesNotThrow(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _expectedResponse.ResourceID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ResourceID), Times.Once);
            AssertUpdateResponseDispatcher(Times.Once());
            AssertInventoryUpdateDispatcher(Times.Once());
        }

        [Test]
        public void Positive_HandleMessages_LootServiceReturnsNothing_NoInventoryUpdatesDispatched()
        {
            SetupRepositoryContains(true, _harvestNode.ResourceID);
            SetupRepositoryGet(_harvestNode);
            SetupLootService(_harvestNode);
            SetupUnlockChecker(true);
            
            Assert.DoesNotThrow(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _expectedResponse.ResourceID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ResourceID), Times.Once);
            AssertUpdateResponseDispatcher(Times.Once());
            AssertInventoryUpdateDispatcher(Times.Never());
        }

        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _updateMediator.HandleMessages(null!));
            
            AssertUpdateResponseDispatcher(Times.Never());
            AssertInventoryUpdateDispatcher(Times.Never());
        }
        
        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _updateMediator.HandleMessages([]));
            
            AssertUpdateResponseDispatcher(Times.Never());
            AssertInventoryUpdateDispatcher(Times.Never());
        }

        [Test]
        public void Negative_HandleMessages_ValidatorReturnsFalse_Throws()
        {
            _skillNodeAccessValidatorMock.Setup(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _nodeUpdate.ResourceID)).Throws<Exception>();
            
            Assert.Throws<Exception>(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            AssertUpdateResponseDispatcher(Times.Never());
            AssertInventoryUpdateDispatcher(Times.Never());
        }

        [Test]
        public void Negative_HandleMessages_IsUnlockedReturnsFalse_Throws()
        {
            SetupUnlockChecker(false);
            
            Assert.Throws<HarvestNodeLockedException>(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            AssertUpdateResponseDispatcher(Times.Never());
            AssertInventoryUpdateDispatcher(Times.Never());
        }
    }
}