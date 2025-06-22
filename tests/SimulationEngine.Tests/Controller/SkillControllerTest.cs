using IdelPog.SimulationEngine.Flows.Skill;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Orchestration;
using IdelPog.SimulationEngine.Structures.Types;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class SkillControllerTest
    {
        private ISkillController _controller { get; set; }
        private Mock<IJobMediator> _jobMediatorMock { get; set; }
        private Job _miningJob { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningJob = JobFactory.CreateMining();
            _jobMediatorMock = new Mock<IJobMediator>();
            _controller = new SkillController(_jobMediatorMock.Object);
        }

        [Test]
        public void Positive_CompleteJob_ReturnsSuccess()
        {
            _jobMediatorMock.Setup(library => library.ProcessJobAction(_miningJob.SkillID))
                .Returns(ServiceResponse.Success);
            
            ServiceResponse serviceResponse = _controller.SwitchSkill(_miningJob.SkillID);
            
            _jobMediatorMock.Verify(library => library.ProcessJobAction(_miningJob.SkillID), Times.Once());
            Assert.That(serviceResponse.IsSuccess, Is.True);
        }

        [Test]
        public void Negative_CompleteJob_Error_ReturnsFailed()
        {
            _jobMediatorMock.Setup(library => library.ProcessJobAction(_miningJob.SkillID))
                .Returns(ServiceResponse.Failure(""));
            
            ServiceResponse serviceResponse = _controller.SwitchSkill(_miningJob.SkillID);
            
            _jobMediatorMock.Verify(library => library.ProcessJobAction(_miningJob.SkillID), Times.Once());
            Assert.That(serviceResponse.IsSuccess, Is.False);
        }
    }
}