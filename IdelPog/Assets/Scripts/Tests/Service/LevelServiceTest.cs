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
            int experienceNeededToLevelUp = _farmingJob.NextLevelExperience;
            _service.LevelUpJob(_farmingJob);

            Assert.AreEqual(1, _farmingJob.Level);
            Assert.AreNotEqual(experienceNeededToLevelUp, _farmingJob.NextLevelExperience);
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
        public void Negative_LevelUpJob_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _service.LevelUpJob(null));
        }
    }
}