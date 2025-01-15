using System;
using IdelPog.Model;
using IdelPog.Service;
using NUnit.Framework;
using Tests.Utils;

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
            _farmingJob = JobFactory.CreateFarming();
            _service = new LevelService();
        }

        [TearDown]
        public void TearDown()
        {
            _farmingJob = JobFactory.CreateFarming();
        }

        [Test]
        public void Positive_LevelUpJob_LevelsJob()
        {
            int experienceNeededToLevelUp = _farmingJob.ExperienceToNextLevel;
            _service.LevelUpJob(_farmingJob);

            Assert.AreEqual(1, _farmingJob.Level);
            Assert.AreNotEqual(experienceNeededToLevelUp, _farmingJob.ExperienceToNextLevel);
        }

        [Test]
        public void Negative_LevelUpJob_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _service.LevelUpJob(null));
        }
    }
}