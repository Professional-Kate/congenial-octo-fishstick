using System;
using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Service;
using IdelPog.Validation;
using IdelPog.Validation.Assertions.Interfaces;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Service
{
    [TestFixture]
    public class ExperienceServiceTest
    {
        private IExperienceService _experienceService { get; set; }
        private Mock<ILevelableAsserter> _levelableAsserterMock { get; set; }

        private Job _miningJob { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelableAsserterMock = new Mock<ILevelableAsserter>(); 
            
            _miningJob = JobFactory.CreateMining();
            _experienceService = new ExperienceService(_levelableAsserterMock.Object);
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
            _levelableAsserterMock.Setup(library => library.AssertLevelable(_miningJob))
                .Throws(new MaxLevelException(_miningJob));
            
            const int experience = 100;
            const int experiencePerAction = 1;
            
            _miningJob.Setup(JobConstants.MAX_JOB_LEVEL, experience, 1, experiencePerAction);
            
            Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(_miningJob));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void Negative_AddExperience_BadExperiencePerAction_Throws(int experiencePerAction)
        {
            _levelableAsserterMock.Setup(library => library.AssertLevelable(_miningJob))
                .Throws(new NegativeNumberException(experiencePerAction));
            
            _miningJob.SetExperiencePerAction(experiencePerAction);
            
            Assert.Throws<NegativeNumberException>(() => _experienceService.AddExperience(_miningJob));
        }

        [Test]
        public void Negative_AddExperience_NullJob_Throws()
        {
            _levelableAsserterMock.Setup(library => library.AssertLevelable(null))
                .Throws(new ArgumentNullException());
            
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null));
        }
    }
}