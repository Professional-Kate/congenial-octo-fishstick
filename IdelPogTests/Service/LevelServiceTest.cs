using IdelPog.Engine.Constants;
using IdelPog.Engine.Models;
using IdelPog.Engine.Service;
using IdelPog.Engine.Validation.Exceptions;
using IdelPog.Engine.Validation.Pipelines;
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
        public void Positive_LevelUpJob_LevelsJob()
        {
            _service.LevelUpJob(_levelable);

            Assert.That(1, Is.EqualTo(_levelable.Level));
        }
        
        [Test]
        public void Positive_CanJobLevel_ReturnsTrue()
        {
            ILevelable levelable = new Levelable(1, 10, 10, 1);

            bool canJobLevel = _service.CanJobLevel(levelable);
            Assert.That(canJobLevel, Is.True);
        }
        
        [Test]
        public void Positive_CanJobLevel_ReturnsFalse()
        {
            ILevelable levelable = new Levelable(1, 5, 10, 1);

            bool canJobLevel = _service.CanJobLevel(levelable);
            Assert.That(canJobLevel, Is.False);
        }


        [TestCase(1, ExpectedResult = 1)]
        [TestCase(5, ExpectedResult = 5)]
        [TestCase(20, ExpectedResult = 20)]
        [TestCase(30, ExpectedResult = 30)]
        public int Positive_LevelUpJob_MultipleTimes(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                _service.LevelUpJob(_levelable);
            }

            return _levelable.Level;
        }

        [Test]
        public void Positive_JobCanLevelToMax()
        {
            Levelable levelable = new(1, 0, 100, 1);
            
            for (int i = 1; i < JobConstants.MAX_JOB_LEVEL; i++)
            {
                levelable.SetExperience(levelable.NextLevelExperience + levelable.Experience); // this is here to sum the total experience
                
                _service.LevelUpJob(levelable);
                
                Console.WriteLine($"LEVEL {levelable.Level} | Experience: {levelable.Experience} | Next Level: {levelable.NextLevelExperience}");
            }
            
            Assert.That(levelable.Level, Is.EqualTo(JobConstants.MAX_JOB_LEVEL));
        }

        [Test]
        public void Negative_LevelUpJob_NullJob_Throws()
        {
            _levelableAsserterMock.Setup(library => library.AssertLevelable(null))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _service.LevelUpJob(null));
        }

        [Test]
        public void Negative_LeveUpJob_MaxLevel_Throws()
        {
            ILevelable levelable = new Levelable(1, 0, 100, 1);
            
            _levelableAsserterMock.Setup(library => library.AssertLevelable(levelable))
                .Throws(new MaxLevelException(levelable.Level));
            
            Assert.Throws<MaxLevelException>(() => _service.LevelUpJob(levelable));
        }
    }
}