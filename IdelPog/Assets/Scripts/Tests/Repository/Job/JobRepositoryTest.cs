using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Repository
{
    [TestFixture]
    public class JobRepositoryTest
    {
        private IJobRepository _repository { get; set; }
        private static readonly JobType _jobType = JobType.MINING;

        private Job _miningJob { get; set; }

        [SetUp]
        public void Setup()
        {
            _repository = new JobRepository();
            _miningJob = JobFactory.CreateMining();
        }
        
        [Test]
        public void Positive_Add_AddsJobIntoRepository()
        {
            _repository.Add(_jobType, _miningJob);

            Job job = _repository.Get(_jobType);
            Assert.AreEqual(_miningJob.JobType, job.JobType);
            Assert.AreEqual(_miningJob.Information.Name, job.Information.Name);
        }
        
        [Test]
        public void Negative_AddWithNullJob_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add(_jobType, null));
        }

        [Test]
        public void Negative_AddWithBadType_ThrowsException()
        {
            Job badJob = new(JobType.NO_TYPE, new Information());
            
            Assert.Throws<NoTypeException>(() => _repository.Add(JobType.NO_TYPE, badJob));
        }

        [Test]
        public void Negative_AddDuplicateJob_ThrowsException()
        {
            _repository.Add(_jobType, _miningJob);
            
            Assert.Throws<ArgumentException>(() => _repository.Add(_jobType, _miningJob));
        }

        [Test]
        public void Positive_Remove_RemovesJobFromRepository()
        {
            _repository.Add(_jobType, _miningJob);
            _repository.Remove(_jobType);
            
            Assert.Throws<NotFoundException>(() => _repository.Get(_jobType));
        }

        [Test]
        public void Positive_Remove_RemovesJobWithCorrectTag()
        {
            _repository.Add(_jobType, _miningJob);
            
            Job farmingJob = JobFactory.CreateFarming();
            _repository.Add(JobType.FARMING, farmingJob);
            
            _repository.Remove(_jobType);
            Job returningJob = _repository.Get(farmingJob.JobType);
            Assert.IsNotNull(returningJob);
        }

        [Test]
        public void Negative_RemoveWithBadType_ThrowsException()
        {
            Assert.Throws<NoTypeException>(() => _repository.Remove(JobType.NO_TYPE));
        }

        [Test]
        public void Negative_RemoveNonExistingJob_ThrowsException()
        {
            Assert.Throws<NotFoundException>(() => _repository.Remove(_jobType));
        }

        [Test]
        public void Positive_Get_ReturnsJob()
        {
            _repository.Add(_jobType, _miningJob);
            
            Job job = _repository.Get(_jobType);
            Assert.AreEqual(_miningJob.JobType, job.JobType);
            Assert.AreEqual(_miningJob.Information.Name, job.Information.Name);
        }

        [Test]
        public void Positive_Get_ReturnsClone()
        {
            _repository.Add(_jobType, _miningJob);
            Job job = _repository.Get(_jobType);

            Assert.AreEqual(_miningJob.JobType, job.JobType);
            Assert.AreNotEqual(_miningJob, job);
        }
        
        
        [Test]
        public void Positive_Get_ReturnsCorrectJob()
        {
            _repository.Add(_jobType, _miningJob);
            
            Job farmingJob = JobFactory.CreateFarming();
            _repository.Add(JobType.FARMING, farmingJob);
            
            Job miningJob = _repository.Get(_jobType);
            Assert.AreEqual(_miningJob.JobType, miningJob.JobType);
            Assert.AreEqual(_miningJob.Information.Name, miningJob.Information.Name);
            Assert.AreNotEqual(farmingJob, miningJob);
        }

        [Test]
        public void Negative_GetWithBadType_ThrowsException()
        {
            Assert.Throws<NoTypeException>(() => _repository.Get(JobType.NO_TYPE));
        }

        [Test]
        public void Negative_GetNonExistingJob_ThrowsException()
        {
            Assert.Throws<NotFoundException>(() => _repository.Get(JobType.MINING));
        }

        [Test]
        public void Positive_Update_UpdatesJob()
        {
            _repository.Add(_jobType, _miningJob);

            const int newExperience = 10000;
            _miningJob.SetExperience(newExperience);
            
            _repository.Update(_jobType, _miningJob);
            
            Assert.AreEqual(newExperience, _miningJob.Experience);
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            Assert.Throws<NotFoundException>(() => _repository.Update(_jobType, _miningJob));
        }

        [Test]
        public void Negative_Update_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Update(_jobType, null));
        }

        [Test]
        public void Negative_Update_BadTag_Throws()
        {
            Assert.Throws<NoTypeException>(() => _repository.Update(JobType.NO_TYPE, _miningJob));
        }
    }
}