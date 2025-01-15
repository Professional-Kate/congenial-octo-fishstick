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
        
        [Test]
        public void Positive_CanJobLevel_ReturnsTrue()
        {
            _miningJob.SetExperiencePerAction(10000);
            
            _experienceService.AddExperience(_miningJob);
            
            bool canJobLevel = _experienceService.CanJobLevel(_miningJob);
            Assert.IsTrue(canJobLevel);
        }
        
        [Test]
        public void Positive_CanJobLevel_ReturnsFalse()
        {
            _miningJob.SetExperiencePerAction(1);
            
            _experienceService.AddExperience(_miningJob);
            
            bool canJobLevel = _experienceService.CanJobLevel(_miningJob);
            Assert.IsFalse(canJobLevel);
        }

        [TestCase(10)]
        [TestCase(1)]
        [TestCase(1000)]
        public void Positive_AddExperience_AddsExperience(int experiencePerAction)
        {
            _miningJob.SetExperiencePerAction(experiencePerAction);
            
            _experienceService.AddExperience(_miningJob);
            
            Assert.AreEqual(experiencePerAction, _miningJob.Experience);
        }
        
        [Test]
        public void Positive_AddExperience_WillCauseLevelUp_ReturnsTrue()
        {
            _miningJob.SetExperiencePerAction(10000);

            _experienceService.AddExperience(_miningJob);
            
            Assert.AreEqual(10000, _miningJob.Experience);
            Assert.AreEqual(1, _miningJob.Level);
        }
        
        [Test]
        public void Negative_AddExperience_MaxLevel_Throws()
        {
            const int experience = 100;
            const int experiencePerAction = 1;
            
            _miningJob.Setup(JobConstants.MAX_JOB_LEVEL, experience, 1, experiencePerAction);
            
            Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(_miningJob));
        }

        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void Negative_AddExperience_BadExperiencePerAction_Throws(int experiencePerAction)
        {
            _miningJob.SetExperiencePerAction(experiencePerAction);
            
            Assert.Throws<ArgumentException>(() => _experienceService.AddExperience(_miningJob));
        }

        [Test]
        public void Negative_AddExperience_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null));
        }
    }
}