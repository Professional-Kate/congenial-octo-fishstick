using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Service
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
            ILevelable levelable = new Levelable(JobConstants.MAX_JOB_LEVEL, 100, 10, 1);
            
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