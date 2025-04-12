using System.Diagnostics;
using IdelPogTemp.Main.Constants;
using IdelPogTemp.Main.Service.Level;
using IdelPogTemp.Main.Structures.Models.Builders.Levelable;
using IdelPogTemp.Main.Structures.Models.Levelable;
using IdelPogTemp.Main.Validation.Exceptions;
using IdelPogTemp.Main.Validation.Pipelines.Interfaces;
using IdelPogTemp.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace IdelPogTemp.Tests.Service
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
            _levelable = LevelableBuilder.Builder()
                .Experience(10)
                .NextLevelExperience(10)
                .Build();

            bool canJobLevel = _service.CanJobLevel(_levelable);
            Assert.That(canJobLevel, Is.True);
        }
        
        [Test]
        public void Positive_CanJobLevel_ReturnsFalse()
        {
            _levelable = LevelableBuilder.Builder()
                .NextLevelExperience(10)
                .Build();

            bool canJobLevel = _service.CanJobLevel(_levelable);
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
            _levelable = LevelableBuilder.Builder()
                .Level(1)
                .Experience(0)
                .NextLevelExperience(100)
                .ExperiencePerAction(1)
                .Build();

            for (int i = 1; i < JobConstants.MAX_JOB_LEVEL; i++)
            {
                _levelable.SetExperience(_levelable.NextLevelExperience + _levelable.Experience); // this is here to sum the total experience
                
                _service.LevelUpJob(_levelable);
                
                Console.WriteLine($"LEVEL {_levelable.Level} | Experience: {_levelable.Experience} | Next Level: {_levelable.NextLevelExperience}");
            }
            
            Assert.That(JobConstants.MAX_JOB_LEVEL, Is.EqualTo(_levelable.Level));
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
            ILevelable levelable = LevelableBuilder.Builder()
                .Level(JobConstants.MAX_JOB_LEVEL)
                .Build();
            
            _levelableAsserterMock.Setup(library => library.AssertLevelable(levelable))
                .Throws(new MaxLevelException(levelable.Level));
            
            Assert.Throws<MaxLevelException>(() => _service.LevelUpJob(levelable));
        }
    }
}