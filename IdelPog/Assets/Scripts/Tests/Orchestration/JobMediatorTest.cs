using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Orchestration;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Orchestration
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
        }

        private void VerifyDependencyCalls(int getCalls = 0, int updateCalls = 0, int serviceCalls = 0, int levelServiceCalls = 0)
        {
            _repositoryMock.Verify(library => library.Get(_miningJob.JobType), Times.Exactly(getCalls));
            _repositoryMock.Verify(library => library.Update(_miningJob.JobType, _miningJob), Times.Exactly(updateCalls));
            _experienceServiceMock.Verify(library => library.AddExperience(_miningJob), Times.Exactly(serviceCalls));
            _levelServiceMock.Verify(library => library.LevelUpJob(_miningJob), Times.Exactly(levelServiceCalls));
        }

        [Test]
        public void Positive_ProcessJobAction_ReturnsSuccess()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningJob));
            
            ServiceResponse response = _jobMediator.ProcessJobAction(_miningJob.JobType);
            
            Assert.True(response.IsSuccess);

            VerifyDependencyCalls(1, 1, 1);
        }

        [Test]
        public void Positive_ProcessJobAction_JobLevelsUp()
        {
            _experienceServiceMock.Setup(library => library.CanJobLevel(_miningJob)).Returns(true);

            ServiceResponse response = _jobMediator.ProcessJobAction(_miningJob.JobType);
            
            Assert.True(response.IsSuccess);
            
            VerifyDependencyCalls(1, 1, 1, 1);
        }


        [Test]
        public void Negative_ExperienceService_ReturnsFailed()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningJob))
                .Throws<ArgumentException>();
            
            ServiceResponse response = _jobMediator.ProcessJobAction(_miningJob.JobType);
            
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Message);
            
            VerifyDependencyCalls(1, 0, 1);
        }

        
        [TestCase(JobType.NO_TYPE, typeof(NoTypeException))]
        [TestCase(JobType.FARMING, typeof(NotFoundException))]
        public void Negative_RepositoryMock_Get_Throws(JobType jobType, Type exception)
        {
            _repositoryMock.Setup(library => library.Get(jobType))
                .Throws((Exception) Activator.CreateInstance(exception));
            
            ServiceResponse response = _jobMediator.ProcessJobAction(jobType);
            
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Message);
            
            VerifyDependencyCalls();
        }

        [TestCase(JobType.NO_TYPE, typeof(NoTypeException))]
        [TestCase(JobType.FARMING, typeof(NotFoundException))]
        public void Negative_RepositoryMock_Update_Throws(JobType jobType, Type exception)
        {
            _repositoryMock.Setup(library => library.Update(jobType, It.IsAny<Job>()))
                .Throws((Exception) Activator.CreateInstance(exception));
            
            ServiceResponse response = _jobMediator.ProcessJobAction(jobType);
            
            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Message);
            
            VerifyDependencyCalls();
        }
    }
}