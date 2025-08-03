using IdelPog.Common.Level;
using IdelPog.Common.Level.Assertions;
using IdelPog.Common.Level.Experience;
using IdelPog.Common.Level.Pipelines;
using IdelPog.Common.Structures;
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
        private Levelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            IHandler throwHandler = new ThrowHandler();
            _experienceService = new ExperienceService(new LevelableAssertionPipeline(new LevelAssertion(throwHandler), new ObjectNullAssertion(throwHandler)));
        }

        [SetUp]
        public void SetUp()
        {
            _levelable = LevelableFactory.CreateLevelable();
        }

        [TestCase(10u)]
        [TestCase(1u)]
        [TestCase(1000u)]
        public void Positive_AddExperience_AddsExperience(uint experiencePerAction)
        {
            _levelable.ExperiencePerAction =  experiencePerAction;

            _experienceService.AddExperience(_levelable);

            Assert.That(experiencePerAction, Is.EqualTo(_levelable.Experience));
        }

        [Test]
        public void Positive_AddExperience_WillCauseLevelUp_ReturnsTrue()
        {
            _levelable.ExperiencePerAction = 10000;

            _experienceService.AddExperience(_levelable);

            Assert.That(_levelable.Experience, Is.EqualTo(_levelable.ExperiencePerAction));
            Assert.That(_levelable.Level, Is.EqualTo(0));
        }

        [Test]
        public void Negative_AddExperience_MaxLevel_Throws()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL, 100, 10, 1);

            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _experienceService.AddExperience(levelable));
            Assert.Multiple(() =>
            {
                Assert.That(exception.ID, Is.EqualTo(levelable));
                Assert.That(exception.SourceName, Is.EqualTo(nameof(levelable)));
            });
        }

        [Test]
        public void Negative_AddExperience_NullSkill_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _experienceService.AddExperience(null!));
        }
    }
}