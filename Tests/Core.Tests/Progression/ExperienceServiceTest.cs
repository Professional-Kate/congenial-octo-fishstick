using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Progression
{
    [TestFixture]
    public sealed class ExperienceServiceTest
    {
        private IExperienceService _experienceService { get; set; }
        private Levelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _experienceService = new ExperienceService(new LevelAssertion(), new ObjectNullAssertion());
        }

        [SetUp]
        public void SetUp()
        {
            _levelable = new Levelable(0, 0, 0, 0);
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

            Assert.Multiple(() =>
            {
                Assert.That(_levelable.Experience, Is.EqualTo(_levelable.ExperiencePerAction));
                Assert.That(_levelable.Level, Is.EqualTo(0));
            });
        }

        [Test]
        public void Negative_AddExperience_AboveMaxLevel_Throws()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL + 1, 100, 10, 1);

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