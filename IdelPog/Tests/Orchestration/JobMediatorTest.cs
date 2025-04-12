using IdelPog.Engine.Orchestration;
using IdelPog.Engine.Repository;
using IdelPog.Engine.Service;
using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;
using IdelPog.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace IdelPog.Tests.Orchestration
{
    [TestFixture]
    public class JobMediatorTest
    {
        private IJobMediator _jobMediator { get; set; }
        private Mock<IExperienceService> _experienceServiceMock { get; set; }
        private Mock<IRepository<JobType, Job>> _repositoryMock { get; set; }
        private Mock<ILevelService> _levelServiceMock { get; set; }
        private Job _miningJob { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningJob = JobFactory.CreateMining();
            
            _experienceServiceMock = new Mock<IExperienceService>();
            _repositoryMock = new Mock<IRepository<JobType, Job>>();
            _levelServiceMock = new Mock<ILevelService>();
            _jobMediator = new JobMediator(_experienceServiceMock.Object, _levelServiceMock.Object, _repositoryMock.Object);

            _repositoryMock.Setup(library => library.Get(_miningJob.JobType)).Returns(_miningJob);
            _repositoryMock.Setup(library => library.Contains(_miningJob.JobType)).Returns(true);
        }

        private void VerifyDependencyCalls(int getCalls = 0, int updateCalls = 0, int serviceCalls = 0, int levelServiceCalls = 0)
        {
            _repositoryMock.Verify(library => library.Get(_miningJob.JobType), Times.Exactly(getCalls));
            _repositoryMock.Verify(library => library.Update(_miningJob.JobType, _miningJob), Times.Exactly(updateCalls));
            _experienceServiceMock.Verify(library => library.AddExperience(_miningJob.Levelable), Times.Exactly(serviceCalls));
            _levelServiceMock.Verify(library => library.LevelUpJob(_miningJob.Levelable), Times.Exactly(levelServiceCalls));
        }

        [Test]
        public void Positive_ProcessJobAction_ReturnsSuccess()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningJob.Levelable));
            
            ServiceResponse response = _jobMediator.ProcessJobAction(_miningJob.JobType);
            
            Assert.That(response.IsSuccess, Is.True);

            VerifyDependencyCalls(1, 1, 1);
        }

        [Test]
        public void Positive_ProcessJobAction_JobLevelsUp()
        {
            _levelServiceMock.Setup(library => library.CanJobLevel(_miningJob.Levelable)).Returns(true);

            ServiceResponse response = _jobMediator.ProcessJobAction(_miningJob.JobType);
            
            Assert.That(response.IsSuccess, Is.True);
            
            VerifyDependencyCalls(1, 1, 1, 1);
        }
        
        [Test]
        public void Negative_ProcessJobAction_Catches_Exception()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningJob.Levelable))
                .Throws<Exception>();
            
            ServiceResponse response = _jobMediator.ProcessJobAction(_miningJob.JobType);
            
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Message, Is.Not.Null);
            
            VerifyDependencyCalls(1, 0, 1);
        }
    }
}