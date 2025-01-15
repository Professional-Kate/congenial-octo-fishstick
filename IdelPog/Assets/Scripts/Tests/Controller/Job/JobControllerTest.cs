using IdelPog.Model;
using IdelPog.Orchestration;
using IdelPog.Structures;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Controller
{
    [TestFixture]
    public class JobControllerTest
    {
        private TestableJobController _controller { get; set; }
        private Mock<IJobMediator> _jobMediatorMock { get; set; }
        private Job _miningJob { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningJob = JobFactory.CreateMining();
            _jobMediatorMock = new Mock<IJobMediator>();
            _controller = new TestableJobController(_jobMediatorMock.Object);
        }

        [Test]
        public void Positive_CompleteJob_ReturnsSuccess()
        {
            _jobMediatorMock.Setup(library => library.ProcessJobAction(_miningJob.JobType))
                .Returns(ServiceResponse.Success);
            
            _controller.CompleteJob(_miningJob.JobType);
            
            _jobMediatorMock.Verify(library => library.ProcessJobAction(_miningJob.JobType), Times.Once());
        }

        [Test]
        public void Negative_CompleteJob_Error_ReturnsFailed()
        {
            _jobMediatorMock.Setup(library => library.ProcessJobAction(_miningJob.JobType))
                .Returns(ServiceResponse.Failure(""));
            
            _controller.CompleteJob(_miningJob.JobType);
            
            _jobMediatorMock.Verify(library => library.ProcessJobAction(_miningJob.JobType), Times.Once());
        }
    }
}