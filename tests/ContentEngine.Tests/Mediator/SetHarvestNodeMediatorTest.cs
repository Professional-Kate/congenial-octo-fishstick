using ContentEngine.Runtime.Mediator;
using ContentEngine.Runtime.Services;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Single;
using Moq;

namespace ContentEngine.Tests.Mediator
{
    [TestFixture]
    public class SetHarvestNodeMediatorTest
    {
        private ISingleMediator<SetHarvestNode> _setMediator;
        private ICurrentResourceProvider  _currentResourceProvider;
        private Mock<ISkillNodeAccessValidator> _skillNodeAccessValidatorMock;
        private Mock<IDispatchOne<SetHarvestNodeResponse>> _setResponseDispatcherMock;
        private Mock<ISetHarvestNodeResponseFactory> _setResponseFactoryMock;

        private SetHarvestNode _setHarvestNode;
        private SetHarvestNodeResponse _expectedResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillNodeAccessValidatorMock = new Mock<ISkillNodeAccessValidator>();
            _setResponseDispatcherMock = new Mock<IDispatchOne<SetHarvestNodeResponse>>();
            _setResponseFactoryMock = new Mock<ISetHarvestNodeResponseFactory>();

            _setHarvestNode = new SetHarvestNode
            {
                ResourceID = ResourceID.STONE,
                SkillID = SkillID.MINING
            };
            
            _expectedResponse = new SetHarvestNodeResponse { SetHarvestNode = _setHarvestNode };
            
            
            CurrentResourceProvider currentResourceProvider = new();
            ICurrentResourceSetter currentResourceSetter = currentResourceProvider;
            _currentResourceProvider = currentResourceProvider;
            
            _setMediator = new SetHarvestNodeMediator(_skillNodeAccessValidatorMock.Object, currentResourceSetter,  _setResponseDispatcherMock.Object, _setResponseFactoryMock.Object);
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
            
            Assert.That(_currentResourceProvider.GetCurrentResource(), Is.EqualTo(_setHarvestNode.ResourceID));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_setHarvestNode.SkillID, _setHarvestNode.ResourceID), Times.Once);
            _setResponseFactoryMock.Verify(library => library.Create(_setHarvestNode), Times.Once);
            _setResponseDispatcherMock.Verify(library => library.Dispatch(_expectedResponse), Times.Once);
        }

        [Test]
        public void Negative_HandleMessage_AssertSkillAllows_Throws_NoSuppress()
        {
            _skillNodeAccessValidatorMock.Setup(library => library.AssertSkillAllows(_setHarvestNode.SkillID, _setHarvestNode.ResourceID))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _setMediator.HandleMessage(_setHarvestNode));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_setHarvestNode.SkillID, _setHarvestNode.ResourceID), Times.Once);
            _setResponseFactoryMock.Verify(library => library.Create(_setHarvestNode), Times.Never);
            _setResponseDispatcherMock.Verify(library => library.Dispatch(_expectedResponse), Times.Never);
        }
    }
}