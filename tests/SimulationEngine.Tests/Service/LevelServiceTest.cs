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
    public class LevelServiceTest
    {
        private ILevelService _service { get; set; }
        private Mock<ILevelableAsserter> _levelableAsserterMock { get; set; }

        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelableAsserterMock = new Mock<ILevelableAsserter>();

            _service = new LevelService(_levelableAsserterMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _levelable = LevelableFactory.CreateLevelable();
        }

        [Test]
        public void Positive_LevelUpSkill_LevelsSkill()
        {
            _service.LevelUpSkill(_levelable);

            Assert.That(1, Is.EqualTo(_levelable.Level));
        }

        [Test]
        public void Positive_CanSkillLevel_ReturnsTrue()
        {
            ILevelable levelable = new Levelable(1, 10, 10, 1);

            bool canSkillLevel = _service.CanSkillLevel(levelable);
            Assert.That(canSkillLevel, Is.True);
        }

        [Test]
        public void Positive_CanSkillLevel_ReturnsFalse()
        {
            ILevelable levelable = new Levelable(1, 5, 10, 1);

            bool canSkillLevel = _service.CanSkillLevel(levelable);
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
                _service.LevelUpSkill(_levelable);
            }

            return _levelable.Level;
        }

        [Test]
        public void Positive_SkillCanLevelToMax()
        {
            Levelable levelable = new(1, 0, 100, 1);

            for (int i = 1; i < SkillConstants.MAX_SKILL_LEVEL; i++)
            {
                levelable.SetExperience(levelable.NextLevelExperience + levelable.Experience); // this is here to sum the total experience

                _service.LevelUpSkill(levelable);
            }

            Assert.That(levelable.Level, Is.EqualTo(SkillConstants.MAX_SKILL_LEVEL));
        }

        [Test]
        public void Negative_LevelUpSkill_NullSkill_Throws()
        {
            _levelableAsserterMock.Setup(library => library.AssertLevelable(null))
                .Throws<ArgumentNullException>();

            Assert.Throws<ArgumentNullException>(() => _service.LevelUpSkill(null));
        }

        [Test]
        public void Negative_LeveUpSkill_MaxLevel_Throws()
        {
            ILevelable levelable = new Levelable(1, 0, 100, 1);

            _levelableAsserterMock.Setup(library => library.AssertLevelable(levelable))
                .Throws(new MaxLevelException(levelable.Level));

            Assert.Throws<MaxLevelException>(() => _service.LevelUpSkill(levelable));
        }
    }
}