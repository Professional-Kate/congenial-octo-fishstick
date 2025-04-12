using IdelPog.Engine.Controller;
using IdelPog.Engine.Orchestration;
using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Structures.Types;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class JobControllerTest
    {
        private IJobController _controller { get; set; }
        private Mock<IJobMediator> _jobMediatorMock { get; set; }
        private Job _miningJob { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningJob = JobFactory.CreateMining();
            _jobMediatorMock = new Mock<IJobMediator>();
            _controller = new JobController(_jobMediatorMock.Object);
        }

        [Test]
        public void Positive_CompleteJob_ReturnsSuccess()
        {
            _jobMediatorMock.Setup(library => library.ProcessJobAction(_miningJob.JobType))
                .Returns(ServiceResponse.Success);
            
            ServiceResponse serviceResponse = _controller.CompleteJob(_miningJob.JobType);
            
            _jobMediatorMock.Verify(library => library.ProcessJobAction(_miningJob.JobType), Times.Once());
            Assert.That(serviceResponse.IsSuccess, Is.True);
        }

        [Test]
        public void Negative_CompleteJob_Error_ReturnsFailed()
        {
            _jobMediatorMock.Setup(library => library.ProcessJobAction(_miningJob.JobType))
                .Returns(ServiceResponse.Failure(""));
            
            ServiceResponse serviceResponse = _controller.CompleteJob(_miningJob.JobType);
            
            _jobMediatorMock.Verify(library => library.ProcessJobAction(_miningJob.JobType), Times.Once());
            Assert.That(serviceResponse.IsSuccess, Is.False);
        }
    }
}