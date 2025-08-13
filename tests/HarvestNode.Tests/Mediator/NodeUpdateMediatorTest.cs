using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public class NodeUpdateMediatorTest
    {
        private ISingleMediator<SkillUpdateResponse> _updateMediator;
        private ICurrentResourceProvider _currentResourceProvider;
        private Mock<ISkillNodeAccessValidator> _skillNodeAccessValidatorMock;
        private Mock<INodeUpdateService> _nodeUpdateServiceMock;
        private Mock<IDispatchOne<HarvestNodeUpdateResponse>> _responseDispatcherMock;
        
        private SkillUpdateResponse _skillUpdateResponse;
        private HarvestNodeUpdateResponse _expectedResponse;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            CurrentResourceProvider currentResourceProvider = new();
            _currentResourceProvider = currentResourceProvider;

            _skillUpdateResponse = new SkillUpdateResponse
            {
                HasLeveled = false,
                LevelProgress = new LevelProgress { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                SkillID = SkillID.MINING
            };

            _expectedResponse = new HarvestNodeUpdateResponse
            {
                HasLeveled = false,
                LevelProgress = new LevelProgress { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                ResourceID = ResourceID.STONE
            };
            
            _skillNodeAccessValidatorMock = new Mock<ISkillNodeAccessValidator>();
            _nodeUpdateServiceMock = new Mock<INodeUpdateService>();
            _responseDispatcherMock = new Mock<IDispatchOne<HarvestNodeUpdateResponse>>();
            
            _updateMediator = new NodeUpdateMediator(_currentResourceProvider, _skillNodeAccessValidatorMock.Object,  _nodeUpdateServiceMock.Object, _responseDispatcherMock.Object);
        }
        
        [SetUp]
        public void Setup()
        {
            _skillNodeAccessValidatorMock.Reset();
            _nodeUpdateServiceMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [Test]
        public void Positive_HandleMessage_UpdateNode_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => _updateMediator.HandleMessage(_skillUpdateResponse));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_skillUpdateResponse.SkillID, _expectedResponse.ResourceID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ResourceID), Times.Once);
            _responseDispatcherMock.Verify(library => library.Dispatch(_expectedResponse), Times.Once);
        }
        
        [Test]
        public void Negative_HandleMessage_ValidatorThrows_NoSuppress()
        {
            _skillNodeAccessValidatorMock.Setup(library => library.AssertSkillAllows(_skillUpdateResponse.SkillID, _expectedResponse.ResourceID))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _updateMediator.HandleMessage(_skillUpdateResponse));
            
            _skillNodeAccessValidatorMock.Verify(library => library.AssertSkillAllows(_skillUpdateResponse.SkillID, _expectedResponse.ResourceID), Times.Once);
            _nodeUpdateServiceMock.Verify(library => library.UpdateHarvestNode(_expectedResponse.ResourceID), Times.Never);
            _responseDispatcherMock.Verify(library => library.Dispatch(_expectedResponse), Times.Never);
        }
    }
}