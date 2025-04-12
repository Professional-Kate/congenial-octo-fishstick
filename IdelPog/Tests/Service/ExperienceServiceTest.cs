using IdelPog.Engine.Constants;
using IdelPog.Engine.Service.Experience;
using IdelPog.Engine.Structures.Models.Levelable;
using IdelPog.Engine.Utilities.Builders.Levelable;
using IdelPog.Engine.Validation.Exceptions;
using IdelPog.Engine.Validation.Pipelines.Interfaces;
using IdelPog.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace IdelPog.Tests.Service
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
            
            Assert.That(experiencePerAction, Is.EqualTo(_levelable.Experience));
        }
        
        [Test]
        public void Positive_AddExperience_WillCauseLevelUp_ReturnsTrue()
        {
            _levelable.SetExperiencePerAction(10000);

            _experienceService.AddExperience(_levelable);
            
            Assert.That(10000, Is.EqualTo(_levelable.Experience));
            Assert.That(0, Is.EqualTo(_levelable.Level));
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