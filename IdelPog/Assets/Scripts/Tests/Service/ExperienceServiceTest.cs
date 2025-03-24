using System;
using IdelPog.Constants;
using IdelPog.Service;
using IdelPog.Structures.Builders;
using IdelPog.Structures.Models.Levelable;
using IdelPog.Validation;
using IdelPog.Validation.Pipelines.Interfaces;
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
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelableAsserterMock = new Mock<ILevelableAsserter>(); 
            _experienceService = new ExperienceService(_levelableAsserterMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _levelable = LevelableFactory.CreateLevelable();
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
            Assert.AreEqual(0, _levelable.Level);
        }
        
        [Test]
        public void Negative_AddExperience_MaxLevel_Throws()
        {
            ILevelable levelable = LevelableBuilder.Builder()
                .Level(JobConstants.MAX_JOB_LEVEL)
                .ExperiencePerAction(1)
                .Experience(100)
                .Build();
            
            _levelableAsserterMock.Setup(library => library.AssertLevelable(levelable))
                .Throws(new MaxLevelException(levelable));
            
            Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(levelable));
        }

        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-1000)]
        public void Negative_AddExperience_BadExperiencePerAction_Throws(int experiencePerAction)
        {
            _levelableAsserterMock.Setup(library => library.AssertLevelable(_levelable))
                .Throws(new NegativeNumberException(experiencePerAction));
            
            _levelable.SetExperiencePerAction(experiencePerAction);
            
            Assert.Throws<NegativeNumberException>(() => _experienceService.AddExperience(_levelable));
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