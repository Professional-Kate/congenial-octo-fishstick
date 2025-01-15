using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Service;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Service
{
    [TestFixture]
    public class ExperienceServiceTest
    {
        private IExperienceService _experienceService { get; set; }
        private Job _miningJob { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _miningJob = JobFactory.CreateMining();
            _experienceService = new ExperienceService();
        }

        [SetUp]
        public void SetUp()
        {
            _miningJob = JobFactory.CreateMining();
            _miningJob.Setup(1, 0, 10000, 1);
        }

        [TestCase(10)]
        [TestCase(1)]
        [TestCase(1000)]
        public void Positive_AddExperience_AddsExperience_ReturnsFalse(int experience)
        {
            bool canLevel = _experienceService.AddExperience(_miningJob, experience);
            
            Assert.False(canLevel);
            Assert.AreEqual(experience, _miningJob.Experience);
        }
        
        [Test]
        public void Positive_AddExperience_WillCauseLevelUp_ReturnsTrue()
        {
            int experienceNeededToLevel = _miningJob.ExperienceToNextLevel;
            bool canLevel = _experienceService.AddExperience(_miningJob, experienceNeededToLevel);
            
            Assert.IsTrue(canLevel);
            Assert.AreEqual(experienceNeededToLevel, _miningJob.Experience);
        }
        
        [Test]
        public void Negative_AddExperience_MaxLevel_Throws()
        {
            const int experience = 100;
            const int experiencePerAction = 1;
            
            _miningJob.Setup(JobConstants.MAX_JOB_LEVEL, experience, 1, experiencePerAction);
            
            Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(_miningJob, experiencePerAction));
        }

        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void Negative_AddExperience_BadExperience_Throws(int experience)
        {
            Assert.Throws<ArgumentException>(() => _experienceService.AddExperience(_miningJob, experience));
        }

        [Test]
        public void Negative_AddExperience_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null, 1));
        }
    }
}