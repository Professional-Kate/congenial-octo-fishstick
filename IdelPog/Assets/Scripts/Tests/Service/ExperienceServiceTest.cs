using System;
using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Service;
using IdelPog.Validation;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Interfaces;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Service
{
    [TestFixture]
    public class ExperienceServiceTest
    {
        private IExperienceService _experienceService { get; set; }
        private Mock<IAssertUnderMaxLevel> _assertUnderMaxLevel { get; set; }
        private Mock<IAssertNotNull> _assertNotNull { get; set; }
        private Mock<IAssertPositive> _assertPositive { get; set; }
        
        private Job _miningJob { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertUnderMaxLevel = new Mock<IAssertUnderMaxLevel>();
            _assertNotNull = new Mock<IAssertNotNull>();
            _assertPositive = new Mock<IAssertPositive>();
            
            _miningJob = JobFactory.CreateMining();
            _experienceService = new ExperienceService(_assertUnderMaxLevel.Object, _assertNotNull.Object, _assertPositive.Object);
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
            _assertUnderMaxLevel.Setup(library => library.AssertLevelIsUnderMax(_miningJob))
                .Throws(new MaxLevelException(_miningJob));
            
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
            _assertPositive.Setup(library => library.AssertNumberIsPositive(experiencePerAction))
                .Throws(new NegativeNumberException(experiencePerAction));
            
            _miningJob.SetExperiencePerAction(experiencePerAction);
            
            Assert.Throws<NegativeNumberException>(() => _experienceService.AddExperience(_miningJob));
        }

        [Test]
        public void Negative_AddExperience_NullJob_Throws()
        {
            _assertNotNull.Setup(library => library.AssertObjectNotNull(null))
                .Throws(new ArgumentNullException());
            
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null));
        }
    }
}