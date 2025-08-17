using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public class SetHarvestNodeMediatorTest
    {
        private ISingleMediator<SetHarvestNode> _setMediator;
        private ICurrentHarvestTargetProvider  _currentHarvestTargetProvider;
        private Mock<ISkillNodeAccessValidator> _skillNodeAccessValidatorMock;
        private Mock<IDispatchOne<SetHarvestNodeResponse>> _setResponseDispatcherMock;
        private Mock<ISetNodeResponseFactory> _setResponseFactoryMock;

        private SetHarvestNode _setHarvestNode;
        private SetHarvestNodeResponse _expectedResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillNodeAccessValidatorMock = new Mock<ISkillNodeAccessValidator>();
            _setResponseDispatcherMock = new Mock<IDispatchOne<SetHarvestNodeResponse>>();
            _setResponseFactoryMock = new Mock<ISetNodeResponseFactory>();

            _setHarvestNode = new SetHarvestNode
            {
                ItemID = ItemID.STONE,
                SkillID = SkillID.MINING
            };
            
            _expectedResponse = new SetHarvestNodeResponse { SetHarvestNode = _setHarvestNode };
            
            
            CurrentHarvestTargetProvider currentHarvestTargetProvider = new();
            ICurrentHarvestTargetSetter currentHarvestTargetSetter = currentHarvestTargetProvider;
            _currentHarvestTargetProvider = currentHarvestTargetProvider;
            
            _setMediator = new SetHarvestNodeMediator(_skillNodeAccessValidatorMock.Object, currentHarvestTargetSetter,  _setResponseDispatcherMock.Object, _setResponseFactoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _skillNodeAccessValidatorMock.Reset();
            _setResponseDispatcherMock.Reset();
            _setResponseFactoryMock.Reset();
        }

        [Test]
        public void Positive_HandleMessage_UpdatesCurrentResource_NoThrow()
        {
            _setResponseFactoryMock.Setup(library => library.Create(_setHarvestNode)).Returns(_expectedResponse);
            
            Assert.DoesNotThrow(() => _setMediator.HandleMessage(_setHarvestNode));
            
            Assert.That(_currentHarvestTargetProvider.GetCurrentHarvestTarget(), Is.EqualTo(_setHarvestNode.ItemID));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_setHarvestNode.SkillID, _setHarvestNode.ItemID), Times.Once);
            _setResponseFactoryMock.Verify(library => library.Create(_setHarvestNode), Times.Once);
            _setResponseDispatcherMock.Verify(library => library.Dispatch(_expectedResponse), Times.Once);
        }

        [Test]
        public void Negative_HandleMessage_AssertSkillAllows_Throws_NoSuppress()
        {
            _skillNodeAccessValidatorMock.Setup(library => library.AssertSkillAllows(_setHarvestNode.SkillID, _setHarvestNode.ItemID))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _setMediator.HandleMessage(_setHarvestNode));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_setHarvestNode.SkillID, _setHarvestNode.ItemID), Times.Once);
            _setResponseFactoryMock.Verify(library => library.Create(_setHarvestNode), Times.Never);
            _setResponseDispatcherMock.Verify(library => library.Dispatch(_expectedResponse), Times.Never);
        }
    }
}