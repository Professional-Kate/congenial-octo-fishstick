using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.Progression.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public sealed class NodeUnlockMediatorTest
    {
        private IBatchMediator<HarvestNodeUnlock> _nodeUnlockMediator;
        private Mock<IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse>> _nodeUnlockerServiceMock;
        private Mock<IDispatchMany<HarvestNodeUnlockResponse>> _dispatcherMock;

        private HarvestNodeUnlock _miningUnlock;
        private HarvestNodeUnlockResponse _miningUnlockResponse;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _nodeUnlockerServiceMock = new Mock<IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse>>();
            _dispatcherMock = new Mock<IDispatchMany<HarvestNodeUnlockResponse>>();
            _nodeUnlockMediator = new NodeUnlockMediator(_nodeUnlockerServiceMock.Object, _dispatcherMock.Object, new CollectionAssertion(new ThrowHandler()));

            _miningUnlock = new HarvestNodeUnlock { SkillID = SkillID.WOOD_CUTTING, SkillLevel = 5 };
            _miningUnlockResponse = new HarvestNodeUnlockResponse { SkillID = SkillID.WOOD_CUTTING, ResourceID = ResourceID.BIRCH_TREE };
        }

        [SetUp]
        public void Setup()
        {
            _nodeUnlockerServiceMock.Reset();
            _dispatcherMock.Reset();
        }

        private void SetupCanUnlock(bool canUnlock, HarvestNodeUnlock harvestNodeUnlock)
        {
            _nodeUnlockerServiceMock.SetupSequence(library => library.CanUnlock(harvestNodeUnlock.SkillID, harvestNodeUnlock.SkillLevel))
                .Returns(canUnlock)
                .Returns(true)
                .Returns(false);
        }

        private void VerifyDispatcherCalled(HarvestNodeUnlockResponse[] harvestNodeUnlockResponses)
        {
            _dispatcherMock.Verify(library => library.Dispatch(harvestNodeUnlockResponses), Times.Once);
        }

        private void VerifyDispatcherNotCalled()
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeUnlockResponse[]>()), Times.Never);
        }
        
        [Test]
        public void Positive_HandleMessages_OneMessage_UnlocksNode()
        {
            SetupCanUnlock(true, _miningUnlock);
            _nodeUnlockerServiceMock.Setup(library => library.UnlockAllAvailable(_miningUnlock.SkillID, _miningUnlock.SkillLevel)).Returns([_miningUnlockResponse]);
            
            Assert.DoesNotThrow(() => _nodeUnlockMediator.HandleMessages([_miningUnlock]));
            
            _nodeUnlockerServiceMock.Verify(library => library.UnlockAllAvailable(_miningUnlock.SkillID, _miningUnlock.SkillLevel), Times.Once);
            _nodeUnlockerServiceMock.Verify(library => library.CanUnlock(_miningUnlock.SkillID, _miningUnlock.SkillLevel), Times.Once);
            _nodeUnlockerServiceMock.VerifyNoOtherCalls();

            VerifyDispatcherCalled([_miningUnlockResponse]);
            _dispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_NoUnlocks_NoDispatch()
        { 
            SetupCanUnlock(false, _miningUnlock);

            Assert.DoesNotThrow(() => _nodeUnlockMediator.HandleMessages([_miningUnlock]));
            
            _nodeUnlockerServiceMock.Verify(library => library.CanUnlock(_miningUnlock.SkillID, _miningUnlock.SkillLevel), Times.Once);
            _nodeUnlockerServiceMock.VerifyNoOtherCalls();
            
            VerifyDispatcherNotCalled();
            _dispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_DispatchesMultipleResponses()
        {
            HarvestNodeUnlock foragingUnlock = _miningUnlock with { SkillID = SkillID.FORAGING };
            HarvestNodeUnlockResponse foragingUnlockResponse = _miningUnlockResponse with { SkillID = SkillID.FORAGING };
            
            SetupCanUnlock(true, _miningUnlock);
            SetupCanUnlock(true, foragingUnlock);
            
            _nodeUnlockerServiceMock.Setup(library => library.UnlockAllAvailable(_miningUnlock.SkillID, _miningUnlock.SkillLevel)).Returns([_miningUnlockResponse]);
            _nodeUnlockerServiceMock.Setup(library => library.UnlockAllAvailable(foragingUnlock.SkillID, foragingUnlock.SkillLevel)).Returns([foragingUnlockResponse]);
            
            Assert.DoesNotThrow(() => _nodeUnlockMediator.HandleMessages([_miningUnlock, foragingUnlock]));
            
            _nodeUnlockerServiceMock.Verify(library => library.CanUnlock(It.IsAny<SkillID>(), It.IsAny<byte>()), Times.Exactly(2));
            _nodeUnlockerServiceMock.Verify(library => library.UnlockAllAvailable(It.IsAny<SkillID>(), It.IsAny<byte>()), Times.Exactly(2));
            _nodeUnlockerServiceMock.VerifyNoOtherCalls();

            VerifyDispatcherCalled([_miningUnlockResponse, foragingUnlockResponse]);
            _dispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_OneMessages_UnlocksMultiple_DispatchesMultiples()
        {
            SetupCanUnlock(true, _miningUnlock);
            
            _nodeUnlockerServiceMock.Setup(library => library.UnlockAllAvailable(_miningUnlock.SkillID, _miningUnlock.SkillLevel)).Returns([_miningUnlockResponse, _miningUnlockResponse]);
            
            Assert.DoesNotThrow(() => _nodeUnlockMediator.HandleMessages([_miningUnlock]));
            
            _nodeUnlockerServiceMock.Verify(library => library.CanUnlock(It.IsAny<SkillID>(), It.IsAny<byte>()), Times.Once);
            _nodeUnlockerServiceMock.Verify(library => library.UnlockAllAvailable(It.IsAny<SkillID>(), It.IsAny<byte>()), Times.Once);
            _nodeUnlockerServiceMock.VerifyNoOtherCalls();

            VerifyDispatcherCalled([_miningUnlockResponse, _miningUnlockResponse]);
            _dispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _nodeUnlockMediator.HandleMessages([]));
            
            VerifyDispatcherNotCalled();
            _nodeUnlockerServiceMock.VerifyNoOtherCalls();
            _dispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _nodeUnlockMediator.HandleMessages(null!));
            
            VerifyDispatcherNotCalled();
            _nodeUnlockerServiceMock.VerifyNoOtherCalls();
            _dispatcherMock.VerifyNoOtherCalls();
        }
    }
}