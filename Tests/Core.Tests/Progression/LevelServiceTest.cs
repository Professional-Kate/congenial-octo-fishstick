using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Progression
{
    [TestFixture]
    public sealed class LevelServiceTest
    {
        private ILevelService _service { get; set; }

        private Levelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _service = new LevelService(new LevelAssertion(), new ObjectNullAssertion());
        }

        [SetUp]
        public void SetUp()
        {
            _levelable = new Levelable(0, 0, 0, 0);
        }

        [Test]
        public void Positive_LevelUpSkill_LevelsSkill()
        {
            _service.LevelUp(_levelable);

            Assert.That(_levelable.Level, Is.EqualTo(1));
        }

        [Test]
        public void Positive_CanSkillLevel_ReturnsTrue()
        {
            Levelable levelable = new(1, 10, 10, 1);

            bool canSkillLevel = _service.CanLevel(levelable);
            Assert.That(canSkillLevel, Is.True);
        }

        [Test]
        public void Positive_CanSkillLevel_ReturnsFalse()
        {
            Levelable levelable = new(1, 5, 10, 1);

            bool canSkillLevel = _service.CanLevel(levelable);
            Assert.That(canSkillLevel, Is.False);
        }


        [TestCase(1, ExpectedResult = 1)]
        [TestCase(5, ExpectedResult = 5)]
        [TestCase(20, ExpectedResult = 20)]
        [TestCase(30, ExpectedResult = 30)]
        public int Positive_LevelUpSkill_MultipleTimes(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                _service.LevelUp(_levelable);
            }

            return _levelable.Level;
        }

        [Test]
        public void Positive_SkillCanLevelToMax()
        {
            Levelable levelable = new(1, 0, 300, 1);

            for (int i = 1; i < LevelConstants.MAX_LEVEL; i++)
            {
                levelable.Experience = levelable.NextLevelExperience + levelable.Experience; // this is here to sum the total experience
                Console.WriteLine(levelable.NextLevelExperience);
                
                _service.LevelUp(levelable);
            }

            Assert.That(levelable.Level, Is.EqualTo(LevelConstants.MAX_LEVEL));
        }

        [Test]
        public void Positive_LevelUp_Increases_NextLevelExperience()
        {
            const uint nextLevelExperience = 300;
            Levelable levelable = new(1, 0, nextLevelExperience, 1);
            
            _service.LevelUp(levelable);
            
            Assert.That(levelable.NextLevelExperience, Is.GreaterThan(nextLevelExperience));
        }

        [Test]
        public void Negative_LevelUpSkill_NullSkill_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _service.LevelUp(null!));
        }

        [Test]
        public void Negative_LeveUpSkill_AboveMaxLevel_Throws()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL + 1, 0, 100, 1);

            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _service.LevelUp(levelable));
            Assert.Multiple(() =>
            {
                Assert.That(exception.ID, Is.EqualTo(levelable));
                Assert.That(exception.SourceName, Is.EqualTo(nameof(levelable)));
            });
        }
    }
}