using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Service;
using IdelPog.Structures.Models.Levelable;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class ExperienceServiceTest
    {
        private IExperienceService _experienceService { get; set; }
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _experienceService = new ExperienceService();
        }

        [SetUp]
        public void SetUp()
        {
            _levelable = new Levelable(1, 0, 10000, 0);
        }
        
        [TestCase(10)]
        [TestCase(1)]
        [TestCase(1000)]
        public void Positive_AddExperience_AddsExperience(int experiencePerAction)
        {
            _levelable.SetExperiencePerAction(experiencePerAction);
            
            _experienceService.AddExperience(_levelable);
            
            Assert.AreEqual(experiencePerAction, _levelable.Experience);
        }
        
        [Test]
        public void Positive_AddExperience_WillCauseLevelUp_ReturnsTrue()
        {
            _levelable.SetExperiencePerAction(10000);

            _experienceService.AddExperience(_levelable);
            
            Assert.AreEqual(10000, _levelable.Experience);
            Assert.AreEqual(1, _levelable.Level);
        }
        
        [Test]
        public void Negative_AddExperience_MaxLevel_Throws()
        {
            const int experience = 100;
            const int experiencePerAction = 1;
            
            _levelable = new Levelable(JobConstants.MAX_JOB_LEVEL, experience, 1, experiencePerAction);
            
            Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(_levelable));
        }

        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void Negative_AddExperience_BadExperiencePerAction_Throws(int experiencePerAction)
        {
            _levelable.SetExperiencePerAction(experiencePerAction);
            
            Assert.Throws<ArgumentException>(() => _experienceService.AddExperience(_levelable));
        }

        [Test]
        public void Negative_AddExperience_NullJob_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null));
        }
    }
}