using IdelPog.SimulationEngine.Assertions;
using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class ExperienceServiceTest
    {
        private IExperienceService _experienceService { get; set; }
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            IHandler throwHandler = new ThrowHandler();
            _experienceService = new ExperienceService(new LevelableAsserter(new AssertUnderMaxLevel(throwHandler), new AssertNotNull(throwHandler), new AssertPositive(throwHandler)));
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
            ILevelable levelable = new Levelable(SkillConstants.MAX_SKILL_LEVEL, 100, 10, 1);
            
            Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(levelable));
        }

        [TestCase(-10)]
        [TestCase(-1000)]
        public void Negative_AddExperience_BadExperiencePerAction_Throws(int experiencePerAction)
        {
            _levelable.SetExperiencePerAction(experiencePerAction);
            
            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() => _experienceService.AddExperience(_levelable));
            
            Assert.That(exception.NumberSource, Is.EqualTo(typeof(ILevelable)));
        }

        [Test]
        public void Negative_AddExperience_NullSkill_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null));
        }
    }
}