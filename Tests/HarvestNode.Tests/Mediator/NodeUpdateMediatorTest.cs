using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Assertion;
using IdelPog.HarvestNode.Exceptions;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Service.Interface;
using IdelPog.Progression.Runtime.Component;
using IdelPog.Progression.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public class NodeUpdateMediatorTest
    {
        private IBatchMediator<HarvestNodeUpdate> _updateMediator;
        private Mock<ISkillNodeAccessValidator> _skillNodeAccessValidatorMock;
        private Mock<INodeUpdateService> _nodeUpdateServiceMock;
        private Mock<IDispatchMany<HarvestNodeUpdateResponse>> _responseDispatcherMock;
        private Mock<ILootService<ItemID>> _lootServiceMock;
        private Mock<IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse>> _checkerMock;
        
        private HarvestNodeUpdate _nodeUpdate;
        private HarvestNodeUpdateResponse _expectedResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _nodeUpdate = new HarvestNodeUpdate
            {
                SkillID = SkillID.MINING,
                ItemID = ItemID.STONE
            };

            _expectedResponse = new HarvestNodeUpdateResponse
            {
                HasLeveled = false,
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                ItemID = ItemID.STONE
            };

            ThrowHandler throwHandler = new();
            _skillNodeAccessValidatorMock = new Mock<ISkillNodeAccessValidator>();
            _nodeUpdateServiceMock = new Mock<INodeUpdateService>();
            _responseDispatcherMock = new Mock<IDispatchMany<HarvestNodeUpdateResponse>>();
            _lootServiceMock = new Mock<ILootService<ItemID>>();
            _checkerMock = new Mock<IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse>>();
            
            _updateMediator = new NodeUpdateMediator(_skillNodeAccessValidatorMock.Object,  _nodeUpdateServiceMock.Object, _responseDispatcherMock.Object, _lootServiceMock.Object, _checkerMock.Object, new NodeUnlockedAssertion(throwHandler), new CollectionAssertion(throwHandler));
        }
        
        [SetUp]
        public void Setup()
        {
            _skillNodeAccessValidatorMock.Reset();
            _nodeUpdateServiceMock.Reset();
            _responseDispatcherMock.Reset();
            _checkerMock.Reset();
        }

        private void SetupUnlockChecker(bool shouldReturn)
        {
            _checkerMock.Setup(library => library.IsUnlocked(_nodeUpdate.SkillID, It.IsAny< Predicate<LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>>>())).Returns(shouldReturn);
        }

        [Test]
        public void Positive_HandleMessage_UpdateNode_DispatchesResponse()
        {
            SetupUnlockChecker(true);
            Assert.DoesNotThrow(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _expectedResponse.ItemID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ItemID), Times.Once);
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<IReadOnlyList<HarvestNodeUpdateResponse>>()), Times.Once);
        }
        
        [Test]
        public void Negative_HandleMessage_ValidatorThrows_NoSuppress()
        {
            SetupUnlockChecker(true);
            _skillNodeAccessValidatorMock.Setup(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _expectedResponse.ItemID))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _expectedResponse.ItemID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ItemID), Times.Never);
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<IReadOnlyList<HarvestNodeUpdateResponse>>()), Times.Never);
        }

        [Test]
        public void Negative_HandleMessages_NodeNotUnlocked_Throws()
        {
            SetupUnlockChecker(false);
            
            Assert.Throws<HarvestNodeLockedException>(() => _updateMediator.HandleMessages([_nodeUpdate]));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_nodeUpdate.SkillID, _expectedResponse.ItemID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ItemID), Times.Never);
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<IReadOnlyList<HarvestNodeUpdateResponse>>()), Times.Never);
        }
    }
}