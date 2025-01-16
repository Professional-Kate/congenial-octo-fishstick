using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Service;
using NUnit.Framework;
using Tests.Utils;
using UnityEngine;

namespace Tests.Service
{
    [TestFixture]
    public class LevelServiceTest
    {
        private ILevelService _service { get; set; }
        private Job _farmingJob { get; set; }
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _service = new LevelService();
        }

        [SetUp]
        public void SetUp()
        {
            _farmingJob = JobFactory.CreateFarming();
        }

        [Test]
        public void Positive_LevelUpJob_LevelsJob()
        {
            _service.LevelUpJob(_farmingJob);

            Assert.AreEqual(1, _farmingJob.Level);
        }
        
        [Test]
        public void Positive_CanJobLevel_ReturnsTrue()
        {
            _farmingJob.Setup(1, 100, 10, 1);

            bool canJobLevel = _service.CanJobLevel(_farmingJob);
            Assert.IsTrue(canJobLevel);
        }
        
        [Test]
        public void Positive_CanJobLevel_ReturnsFalse()
        {
            _farmingJob.Setup(1, 5, 10, 1);

            bool canJobLevel = _service.CanJobLevel(_farmingJob);
            Assert.IsFalse(canJobLevel);
        }


        [TestCase(1, ExpectedResult = 1)]
        [TestCase(5, ExpectedResult = 5)]
        [TestCase(20, ExpectedResult = 20)]
        [TestCase(30, ExpectedResult = 30)]
        public int Positive_LevelUpJob_MultipleTimes(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                _service.LevelUpJob(_farmingJob);
            }

            return _farmingJob.Level;
        }

        [Test]
        public void Positive_JobCanLevelToMax()
        {
            _farmingJob.Setup(1, 0, 83, 1);

            for (int i = 1; i < JobConstants.MAX_JOB_LEVEL; i++)
            {
                _farmingJob.AddExperience(_farmingJob.NextLevelExperience); // this is here to sum the total experience
                
                _service.LevelUpJob(_farmingJob);
                
                Debug.Log($"LEVEL {_farmingJob.Level} | Experience: {_farmingJob.Experience} | Next Level: {_farmingJob.NextLevelExperience}");
            }
            
            Assert.AreEqual(JobConstants.MAX_JOB_LEVEL, _farmingJob.Level);
        }

        [Test]
        public void Negative_LevelUpJob_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _service.LevelUpJob(null));
        }

        [Test]
        public void Negative_LeveUpJob_MaxLevel_Throws()
        {
            _farmingJob.Setup(100, 100, 10, 1);
            Assert.Throws<MaxLevelException>(() => _service.LevelUpJob(_farmingJob));
        }
    }
}